using System;
using Entities;
using Interfaces.Repositories;
using Interfaces.Services;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class SolarSystemsService : ISolarSystemsService
    {
        private IEliteBgsRepository _eliteBgsRepository;

        public SolarSystemsService(IEliteBgsRepository eliteBgsRepository)
        {
            _eliteBgsRepository = eliteBgsRepository;
        }

        public async Task<SolarSystem> Get(string systemName)
        {
            return await _eliteBgsRepository.GetSolarSystem(systemName);
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
                else if (system.SubFactions.Count > 6 && system.SubFactions.Any(e => !e.Name.Contains(system.Name, StringComparison.InvariantCultureIgnoreCase) && e.Influence < 10))
                {
                    var report = new InvasionReport();
                    report.SystemName = system.Name;
                    if (system.UpdatedOn < expansionReport.LastTick)
                        report.TicksOld = "Up to date";
                    else
                        report.TicksOld = "Information is " + ((system.UpdatedOn - expansionReport.LastTick).Days + 1) + " ticks old.";
                    report.ControllingFaction = system.ControllingFaction;
                    var targetFaction = system.SubFactions.OrderBy(e => e.Influence).FirstOrDefault(e => !e.Name.Contains(system.Name, StringComparison.InvariantCultureIgnoreCase));
                    report.TargetFaction = targetFaction.Name;
                    report.Influence = Math.Round(targetFaction.Influence*100,2);
                    expansionReport.InvasionTargets.Add(report);
                }
            }
            
            return expansionReport;
        }
    }
}
