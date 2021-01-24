using System;
using Entities;
using Interfaces.Repositories;
using Interfaces.Services;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Services
{
    public class SolarSystemsService : ISolarSystemsService
    {
        private IEliteBgsRepository _eliteBgsRepository;
        private IEddbRepository _eddbRepository;

        public SolarSystemsService(IEliteBgsRepository eliteBgsRepository, IEddbRepository eddbRepository)
        {
            _eliteBgsRepository = eliteBgsRepository;
            _eddbRepository = eddbRepository;
        }

        public async Task<List<SolarSystem>> GetByFactionName(string factionName)
        {
            var solarSystems = await _eliteBgsRepository.GetSolarSystemByFactionName(factionName);
            foreach (var system in solarSystems)
            {
                var homeFactions = await _eddbRepository.GetHomeFactions(system.Name);
                foreach (var subFaction in system.SubFactions)
                {
                    if (homeFactions.Contains(subFaction.Name, StringComparer.InvariantCultureIgnoreCase))
                        subFaction.HomeSystem = true;
                    else
                        subFaction.HomeSystem = false;
                }
            }
            return solarSystems;
        }

        public async Task<ExpansionReport> GetExpansionTargets(ExpansionReport expansionReport)
        {
            var expansionSystem = await _eliteBgsRepository.GetSolarSystem(expansionReport.ExpandFromSystem);
            var systems = await _eliteBgsRepository.GetExpansionTargets(expansionReport.ExpandFromSystem);

            foreach (var system in systems)
            {
                // skip systems we're already in
                if (system.SubFactions.Any(e => e.Name.ToLower() == expansionSystem.ControllingFaction.ToLower()))
                    continue;

                if (system.SubFactions.Count > 2 && system.SubFactions.Count < 7)
                {
                    expansionReport.NeverRetreatedSystemsWithSpace.Add(system);
                }
                else if(system.SubFactions.Count() == 7)
                {
                    var homeFactions = await _eddbRepository.GetHomeFactions(system.Name);
                    foreach (var subFaction in system.SubFactions)
                    {
                        if (homeFactions.Contains(subFaction.Name, StringComparer.InvariantCultureIgnoreCase))
                            subFaction.HomeSystem = true;
                        else
                            subFaction.HomeSystem = false;
                    }

                    if (system.SubFactions.Any(e => !e.HomeSystem && e.Influence < 10))
                    {
                        var report = new InvasionReport();
                        report.SystemName = system.Name;
                        if (system.UpdatedOn > expansionReport.LastTick)
                            report.TicksOld = "Up to date";
                        else
                            report.TicksOld = "Information is " + ((expansionReport.LastTick - system.UpdatedOn).Days + 1) + " ticks old.";
                        report.ControllingFaction = system.ControllingFaction;
                        var targetFaction = system.SubFactions.OrderBy(e => e.Influence).FirstOrDefault(e => !e.HomeSystem);
                        report.TargetFaction = targetFaction.Name;
                        report.Influence = Math.Round(targetFaction.Influence * 100, 2);
                        expansionReport.InvasionTargets.Add(report);
                    }
                }
            }
            
            return expansionReport;
        }
    }
}
