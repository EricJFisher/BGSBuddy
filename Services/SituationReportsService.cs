using Entities;
using Interfaces.Services;
using System;
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

            faction.SolarSystems = await _solarSystemsService.GetByFactionName(situationReport.FactionName);

            // Get Each System's Assets
            foreach (var solarSystem in faction.SolarSystems)
            {
                // Update system assets
                solarSystem.Assets = await _assetsService.GetAssets(solarSystem.Name);
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
                bool inConflict = false;
                bool closeToConflict = false;
                bool closeToExpansion = false;
                bool closeToRetreat = false;
                string states = string.Empty;

                if (system.ControllingFaction.Equals(situationReport.FactionName, StringComparison.OrdinalIgnoreCase))
                    weControl = true;
                if (influences[0] - 0.10 <= influences[1])
                    closeToConflict = true;
                if (system.Conflicts.Any(e => e.Factions.Any(f => f.FactionName.ToLower() == situationReport.FactionName.ToLower())))
                    inConflict = true;
                if (influences[0] <= 0.075)
                    closeToRetreat = true;
                else if (influences[0] >= 0.65)
                    closeToExpansion = true;
                if (system.Assets.Any(e => e.Faction.ToLower() != situationReport.FactionName.ToLower()))
                    totalControl = false;
                if (!string.IsNullOrEmpty(system.State))
                    states = system.State;

                // Stale Data
                if (system.UpdatedOn <= tick.AddDays(-2))
                    situationReport.DataReports.Add(new Report(system.Name, "Stale Data", "System info is behind by roughly " + (tick - system.UpdatedOn).Days.ToString() + " ticks", states));
                else if (system.UpdatedOn < tick)
                    situationReport.DataReports.Add(new Report(system.Name, "Stale Data", "System info is behind by at least 1 tick.", states));

                // In or Pending Conflict
                if (inConflict)
                {
                    var conflict = system.Conflicts.FirstOrDefault(e => e.Factions.Any(f => f.FactionName == situationReport.FactionName));
                    situationReport.CriticalReports.Add(new Report(system.Name, conflict.Type, conflict.Status, "Asset at stake: " + conflict.Factions.Any(e => e.Stake != string.Empty )));
                }
                // Asset Reallocation opportunity
                else if (!totalControl && closeToConflict)
                    situationReport.OpportunityReports.Add(new Report(system.Name, "Asset Reallocation Opportunity", "inf gap : " + Math.Round(influences[0] - influences[1], 2).ToString("p"), states));
                // Pointless conflict risk
                else if (closeToConflict)
                    situationReport.WarningReports.Add(new Report(system.Name, "Pointless Conflict Risk", "inf gap : " + Math.Round(influences[0] - influences[1], 2).ToString("p"), states));

                // Expansion warning
                if (closeToExpansion && weControl)
                    situationReport.WarningReports.Add(new Report(system.Name, "System is nearing expansion.", "current influence : " + Math.Round(influences[0], 2).ToString("p"), states));
                // Near retreat warning
                else if(closeToRetreat)
                    situationReport.WarningReports.Add(new Report(system.Name, "System is nearing retreat.", "current influence : " + Math.Round(influences[0], 2).ToString("p"), states));

                // Total Control
                if (totalControl)
                    situationReport.ControlledReports.Add(new Report(system.Name, "Total Control", system.Assets.Count + " assets controlled.", states));
                // Unclaimed Assets
                else
                    situationReport.PartialReports.Add(new Report(system.Name, "Unclamed Assets", system.Assets.Count(e => e.Faction.ToLower() != situationReport.FactionName.ToLower()) + " of " + system.Assets.Count + " assets unclaimed.", states));

                // Conquest opportunity
                if (!weControl && !inConflict)
                    situationReport.OpportunityReports.Add(new Report(system.Name, "Conquest Opportunity", "inf gap : " + Math.Round(influences[0] - influences[1], 2).ToString("p"), states));

                // Subfaction considerations
                foreach (var subFaction in system.SubFactions)
                {
                    // We're in retreat
                    if (subFaction.Name == faction.Name && subFaction.ActiveStates.Exists(e => e.ToLower() == "retreat"))
                        situationReport.CriticalReports.Add(new Report(system.Name, "Retreat", "We're in retreat!!!", states));

                    // Attempt to guess if someone is near retreat who's not a "home" faction
                    if (subFaction.Name != faction.Name && !subFaction.HomeSystem && subFaction.Influence < 0.05 && !subFaction.ActiveStates.Exists(e => e.ToLower() == "retreat") && !subFaction.PendingStates.Exists(e => e.ToLower() == "retreat"))
                        situationReport.WarningReports.Add(new Report(system.Name, "Retreat Risk Warning", subFaction.Name + " is close to retreat", states));

                    // Other faction is in retreat
                    if (subFaction.Name != faction.Name && !subFaction.HomeSystem && (subFaction.ActiveStates.Exists(e => e.ToLower() == "retreat" || subFaction.PendingStates.Exists(e => e.ToLower() == "retreat"))))
                        situationReport.CriticalReports.Add(new Report(system.Name, "Retreat Warning", subFaction.Name + " is in retreat.", states));


                    // Non-native faction in conflict
                    if (!subFaction.HomeSystem && system.Conflicts.Any(e => e.Status == "active" && e.Factions.Any(f => f.FactionName.ToLower() == subFaction.Name.ToLower()))) 
                        situationReport.WarningReports.Add(new Report(system.Name, "Non-native Conflict Warning", "Non-native faction " + subFaction.Name + " is in active conflict.", states));    
                }
            }

            return situationReport;
        }
    }
}
