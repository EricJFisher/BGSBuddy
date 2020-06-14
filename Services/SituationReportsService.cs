﻿using Entities;
using Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class SituationReportsService : ISituationReportsService
    {
        private readonly IAssetsService _assetsService;
        private readonly IFactionsService _factionsService;
        private readonly ISolarSystemsService _solarSystemsService;
        private readonly ITickService _tickService;

        public SituationReportsService(IAssetsService assetsService, IFactionsService factionsService, ISolarSystemsService solarSystemsService, ITickService tickService)
        {
            _assetsService = assetsService;
            _factionsService = factionsService;
            _solarSystemsService = solarSystemsService;
            _tickService = tickService;
        }

        public async Task<SituationReport> GenerateReport(SituationReport situationReport)
        {
            // Get Tick
            var tick = await _tickService.Get();
            Faction faction = situationReport.Faction;
            // Get Faction
            if (situationReport.Faction.UpdatedOn < tick)
                faction = await _factionsService.Get(situationReport.FactionName);

            // Get Each System and it's Assets
            foreach (var solarSystem in faction.SolarSystems.Where(e => e.UpdatedOn < tick || e.SubFactions.Count < 5).ToList())
            {
                // Update system information
                var temp = await _solarSystemsService.Get(solarSystem.Name);
                solarSystem.Name = temp.Name;
                solarSystem.ControllingFaction = temp.ControllingFaction;
                solarSystem.Conflicts = temp.Conflicts;
                solarSystem.ActiveStates = temp.ActiveStates;
                solarSystem.PendingStates = temp.PendingStates;
                solarSystem.State = temp.State;
                solarSystem.SubFactions = temp.SubFactions;

                // Update system assets
                solarSystem.Assets = await _assetsService.GetAssets(solarSystem.Name);
            }

            var factionsRun = new List<string>();
            // Get Influence for each faction that doesn't have influence / state info
            foreach (var solarSystem in faction.SolarSystems.ToList())
            {
                var test = solarSystem.SubFactions;
                foreach (var subFaction in solarSystem.SubFactions.Where(e => e.UpdatedOn == null || e.UpdatedOn < tick).ToList())
                {
                    if (factionsRun.Contains(subFaction.Name))
                        continue;
                    else
                        factionsRun.Add(subFaction.Name);

                    var tempFaction = await _factionsService.Get(subFaction.Name);

                    foreach (var result in tempFaction.SolarSystems)
                    {
                        if (!faction.SolarSystems.Any(e => e.Name == result.Name))
                            continue;

                        var thisFaction = result.SubFactions.FirstOrDefault(e => e.Name == tempFaction.Name);
                        var update = faction.SolarSystems.FirstOrDefault(e => e.Name == result.Name).SubFactions.FirstOrDefault(s => s.Name == subFaction.Name);
                        update.Name = tempFaction.Name;
                        update.ActiveStates = thisFaction.ActiveStates;
                        update.PendingStates = thisFaction.PendingStates;
                        update.Influence = thisFaction.Influence;
                        update.UpdatedOn = thisFaction.UpdatedOn;
                    }
                }
                test = solarSystem.SubFactions;
            }

            situationReport.Faction = faction;

            // Generate Reports

            foreach (var system in faction.SolarSystems)
            {
                if (situationReport.OffLimits.Any(e => e.ToLower() == system.Name.ToLower()))
                    continue;

                var influences = system.SubFactions.OrderByDescending(e => e.Influence).Select(e => e.Influence).ToList();

                bool weControl = false;
                bool totalControl = true;
                bool closeToConflict = false;
                string states = string.Empty;

                if (system.ControllingFaction.Equals(situationReport.FactionName, StringComparison.OrdinalIgnoreCase))
                    weControl = true;
                if (influences[0] - 0.10 <= influences[1])
                    closeToConflict = true;
                if (system.Assets.Any(e => e.Faction.ToLower() != situationReport.FactionName.ToLower()))
                    totalControl = false;
                if (!string.IsNullOrEmpty(system.State))
                    states = system.State;

                // Stale Data
                if (system.UpdatedOn <= DateTime.UtcNow.AddDays(-2))
                    situationReport.WarningReports.Add(new Report(system.Name, "Stale Data", "Over " + (DateTime.UtcNow - system.UpdatedOn).Days.ToString() + " days old", states));

                // In or Pending Conflict
                if (system.Conflicts.Any(e => e.Factions.Any(f => f.FactionName == situationReport.FactionName)))
                {
                    var conflict = system.Conflicts.FirstOrDefault(e => e.Factions.Any(f => f.FactionName == situationReport.FactionName));
                    situationReport.CriticalReports.Add(new Report(system.Name, conflict.Type, conflict.Status, "Asset at stake: " + conflict.Factions.Any(e => e.Stake != string.Empty )));
                }

                // Asset Reallocation opportunity
                if (!totalControl && closeToConflict)
                    situationReport.OpportunityReports.Add(new Report(system.Name, "Asset Reallocation Opportunity", system.Assets.FirstOrDefault(e => e.Faction.ToLower() != situationReport.FactionName.ToLower()).Faction, states));

                // Pointless conflict risk
                else if (closeToConflict && system.Conflicts.Any(e => e.Factions.Any(f => f.Stake == string.Empty)))
                    situationReport.WarningReports.Add(new Report(system.Name, "Pointless Conflict Risk", "inf gap : " + Math.Round(influences[0] - influences[1], 2).ToString("p"), states));

                // Total Control
                if (totalControl)
                    situationReport.ControlledReports.Add(new Report(system.Name, "Total Control", system.Assets.Count + " assets controlled.", states));
                // Unclaimed Assets
                else
                    situationReport.PartialReports.Add(new Report(system.Name, "Unclamed Assets", system.Assets.Count(e => e.Faction.ToLower() != situationReport.FactionName.ToLower()) + " of " + system.Assets.Count + " assets unclaimed.", states));

                // Conquest opportunity
                if (!weControl)
                    situationReport.OpportunityReports.Add(new Report(system.Name, "Conquest Opportunity", "inf gap : " + Math.Round(influences[0] - influences[1], 2).ToString("p"), states));

                // Subfaction considerations
                foreach (var subFaction in system.SubFactions)
                {
                    // We're in retreat
                    if (subFaction.Name == faction.Name && subFaction.ActiveStates.Exists(e => e.ToLower() == "retreat"))
                        situationReport.CriticalReports.Add(new Report(system.Name, "Retreat", "We're in retreat!!!", states));

                    // Other faction is in retreat
                    if (subFaction.Name != faction.Name && subFaction.ActiveStates.Exists(e => e.ToLower() == "retreat"))
                        situationReport.OpportunityReports.Add(new Report(system.Name, "Retreat Opportunity", subFaction.Name + " is in retreat.", states));
                }
            }

            return situationReport;
        }
    }
}
