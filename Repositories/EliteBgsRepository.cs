using Entities;
using Interfaces.Repositories;
using Repositories.EliteBgsTypes.FactionRequest;
using Repositories.EliteBgsTypes.StationRequest;
using Repositories.EliteBgsTypes.SystemRequest;
using Repositories.EliteBgsTypes.TickRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;
using Faction = Entities.Faction;

namespace Repositories
{
    public class EliteBgsRepository : IEliteBgsRepository
    {
        private readonly string baseUrl = "https://elitebgs.app/api/ebgs/v5/";
        private HttpClient client = new HttpClient();

        public EliteBgsRepository()
        {
        }

        public async Task<DateTime> GetTick()
        {
            // Get Tick from EliteBGS
            var json = await FetchTick();

            // Convert Json to Tick's DateTime
            return ConvertJsonToTick(json);
        }

        private async Task<string> FetchTick()
        {
            var url = baseUrl + "ticks";
            return await client.GetStringAsync(url).ConfigureAwait(false);
        }

        private DateTime ConvertJsonToTick(string json)
        {
            var request = EliteBgsTickRequest.FromJson(json);
            return request[0].Time.UtcDateTime;
        }

        public async Task<List<Asset>> GetAssets(string systemName)
        {
            // Get Asset json from EliteBGS
            var json = await FetchAsset(systemName);

            // Convert json to Assets
            return ConvertJsonToAssets(json);
        }

        private async Task<string> FetchAsset(string systemName)
        {
            var url = baseUrl + "stations?system=" + HttpUtility.UrlEncode(systemName);
            return await client.GetStringAsync(url).ConfigureAwait(false);
        }

        private List<Asset> ConvertJsonToAssets(string json)
        {
            var assets = new List<Asset>();

            var request = EliteBgsStationRequest.FromJson(json);
            foreach (var item in request.Docs)
            {
                var asset = new Asset();
                asset.Name = item.Name;
                asset.SolarSystem = item.System;
                asset.Faction = item.ControllingMinorFaction;
                asset.UpdatedOn = item.UpdatedAt.UtcDateTime;
                assets.Add(asset);
            }
            return assets;
        }

        public async Task<SolarSystem> GetSolarSystem(string systemName)
        {
            // Get Solar System from EliteBGS
            var json = await FetchSolarSystem(systemName);

            // Convert Json to Solar System
            return await ConvertJsonToSolarSystem(json);
        }

        private async Task<string> FetchSolarSystem(string name)
        {
            var url = baseUrl + "systems?name=" + HttpUtility.UrlEncode(name);
            return await client.GetStringAsync(url).ConfigureAwait(false);
        }

        private async Task<SolarSystem> ConvertJsonToSolarSystem(string json)
        {
            var system = new SolarSystem();

            var request = EliteBgsSystemRequest.FromJson(json);
            system.Name = request.Docs[0].Name;
            system.ControllingFaction = request.Docs[0].ControllingMinorFaction;
            system.UpdatedOn = request.Docs[0].UpdatedAt.UtcDateTime;
            if (!string.IsNullOrEmpty(request.Docs[0].State))
                system.State = request.Docs[0].State;
            foreach (var subFaction in request.Docs[0].Factions)
            {
                var faction = new SubFaction();
                faction.Name = subFaction.Name;
                if (subFaction?.FactionDetails?.FactionPresence?.Influence != null)
                    faction.Influence = subFaction.FactionDetails.FactionPresence.Influence;
                else
                    faction.Influence = 0;
                system.SubFactions.Add(faction);
            }
            foreach (var conflict in request.Docs[0].Conflicts)
            {
                system.Conflicts.Add(new Entities.Conflict { 
                    Status = conflict.Status,
                    Type = conflict.Type,
                    Factions = new List<ConflictFaction> { 
                        new ConflictFaction { 
                            DaysWon = (int)conflict.Faction1.DaysWon,
                            FactionName = conflict.Faction1.Name,
                            Stake = conflict.Faction1.Stake
                        },
                        new ConflictFaction
                        {
                            DaysWon = (int)conflict.Faction2.DaysWon,
                            FactionName = conflict.Faction2.Name,
                            Stake = conflict.Faction2.Stake
                        }
                    }
                });
            }
            return system;
        }

        public async Task<List<SolarSystem>> GetExpansionTargets(string solarSystem)
        {
            var systems = new List<SolarSystem>();
            for (int i = 1; i < 7; i++)
            {
                var json = await FetchExpansionTargets(solarSystem, i);
                systems.AddRange(await ConvertJsonToListSolarSystem(json));
            }
            return systems;
        }

        public async Task<string> FetchExpansionTargets(string solarSystem, int page)
        {
            var url = @"https://elitebgs.app/api/ebgs/v5/systems?referenceSystem=" + HttpUtility.UrlEncode(solarSystem) + "&referenceDistance=20&factionDetails=true&page=" + page;
            return await client.GetStringAsync(url).ConfigureAwait(false);
        }

        public async Task<List<SolarSystem>> ConvertJsonToListSolarSystem(string json)
        {
            var solarSystems = new List<SolarSystem>();
            var request = EliteBgsSystemRequest.FromJson(json);
            foreach (var record in request.Docs)
            {
                var system = new SolarSystem();
                system.Name = record.Name;
                system.ControllingFaction = record.ControllingMinorFaction;
                system.UpdatedOn = record.UpdatedAt.UtcDateTime;
                if (!string.IsNullOrEmpty(record.State))
                    system.State = record.State;
                foreach (var subFaction in record.Factions)
                    system.SubFactions.Add(new SubFaction { Name = subFaction.Name, Influence = subFaction.FactionDetails.FactionPresence.Influence });
                foreach (var conflict in record.Conflicts)
                {
                    system.Conflicts.Add(new Entities.Conflict
                    {
                        Status = conflict.Status,
                        Type = conflict.Type,
                        Factions = new List<ConflictFaction> {
                        new ConflictFaction {
                            DaysWon = (int)conflict.Faction1.DaysWon,
                            FactionName = conflict.Faction1.Name,
                            Stake = conflict.Faction1.Stake
                        },
                        new ConflictFaction
                        {
                            DaysWon = (int)conflict.Faction2.DaysWon,
                            FactionName = conflict.Faction2.Name,
                            Stake = conflict.Faction2.Stake
                        }
                    }
                    });
                }
                solarSystems.Add(system);
            }
            return solarSystems;
        }

        public async Task<Faction> GetFaction(string factionName)
        {
            // Get Faction from EliteBgs
            var json = await  FetchFaction(factionName);

            // Convert json to Faction
            return await ConvertJsonToFaction(json);
        }

        private async Task<string> FetchFaction(string name)
        {
            var url = baseUrl + "factions?name=" + HttpUtility.UrlEncode(name);
            return await client.GetStringAsync(url).ConfigureAwait(false);
        }

        private async Task<Faction> ConvertJsonToFaction(string json)
        {
            var faction = new Faction();

            var request = EliteBgsFactionRequest.FromJson(json);
            if (request.Docs.Count == 0)
                return faction;
            faction.Name = request.Docs[0].Name;
            faction.UpdatedOn = request.Docs[0].UpdatedAt.UtcDateTime;

            foreach (var system in request.Docs[0].FactionPresence)
            {
                var solarSystem = new SolarSystem();
                solarSystem.Name = system.SystemName;
                foreach (var conflict in system.Conflicts)
                {
                    solarSystem.Conflicts.Add(new Entities.Conflict
                    {
                        Status = conflict.Status,
                        Type = conflict.Type,
                        Factions = new List<ConflictFaction>
                        {
                            new ConflictFaction{
                                DaysWon = (int)conflict.DaysWon,
                                FactionName = faction.Name,
                                Stake = conflict.Stake
                            },
                            new ConflictFaction
                            {
                                FactionName = conflict.OpponentName
                            }
                        }
                    });
                }
                solarSystem.UpdatedOn = system.UpdatedAt.UtcDateTime;
                solarSystem.ActiveStates = system.ActiveStates.Select(e => e.State).ToList();
                solarSystem.PendingStates = system.PendingStates.Select(e => e.State).ToList();
                solarSystem.State = system.State;

                var subFaction = new SubFaction();
                subFaction.Name = faction.Name;
                subFaction.Influence = system.Influence;
                subFaction.ActiveStates = system.ActiveStates.Select(e => e.State).ToList();
                subFaction.PendingStates = system.PendingStates.Select(e => e.State).ToList();
                subFaction.UpdatedOn = system.UpdatedAt.UtcDateTime;
                solarSystem.SubFactions.Add(subFaction);

                faction.SolarSystems.Add(solarSystem);
            }
            return faction;
        }
    }
}
