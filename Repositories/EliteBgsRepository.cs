using Entities;
using Interfaces.Repositories;
using Repositories.EliteBgsTypes.FactionRequest;
using Repositories.EliteBgsTypes.StationRequest;
using Repositories.EliteBgsTypes.SystemRequest;
using Repositories.EliteBgsTypes.TickRequest;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Faction = Entities.Faction;

namespace Repositories
{
    public class EliteBgsRepository : IEliteBgsRepository
    {
        private readonly string baseUrl = "https://elitebgs.app/api/ebgs/v4/";
        private HttpClient client = new HttpClient();
        private EddbRepository _eddbRepository;
        private FileSystemRepository _fileSystemRepository;

        public EliteBgsRepository()
        {
            _eddbRepository = new EddbRepository();
            _fileSystemRepository = new FileSystemRepository();
        }

        public async Task<Faction> GetFaction(string name, DateTime lastTick, bool forceUpdate = false)
        {
            var file = "F" + name + ".log";
            var json = _fileSystemRepository.RetrieveJsonFromFile(file);
            if(string.IsNullOrEmpty(json) || forceUpdate)
                json = await FetchFaction(name).ConfigureAwait(false);
            var faction = await ConvertJsonToFaction(json, lastTick).ConfigureAwait(false);
            if (faction.UpdatedOn < lastTick && !forceUpdate)
                faction = await GetFaction(name, lastTick, true).ConfigureAwait(false);
            return faction;
        }

        public async Task<string> FetchFaction(string name)
        {
            var file = "F" + name + ".log";
            var url = baseUrl + "factions?name=" + HttpUtility.UrlEncode(name);
            var response = await client.GetStringAsync(url).ConfigureAwait(false);
            _fileSystemRepository.SaveJsonToFile(response, file);
            return response;
        }

        private async Task<Faction> ConvertJsonToFaction(string json, DateTime lastTick)
        {
            var faction = new Faction();

            var request = EliteBgsFactionRequest.FromJson(json);
            if (request.Docs.Count == 0)
                return faction;
            faction.Name = request.Docs[0].Name;
            faction.UpdatedOn = request.Docs[0].UpdatedAt.UtcDateTime;
            
            foreach(var system in request.Docs[0].FactionPresence)
            {
                var solarSystem = new SolarSystem();
                solarSystem.Name = system.SystemName;
                if (system.Conflicts.Count > 0)
                {
                    solarSystem.ConflictType = system.Conflicts[0].Type;
                    solarSystem.ConflictStatus = system.Conflicts[0].Status;
                }

                var stationRequest = await GetSolarSystem(system.SystemName).ConfigureAwait(false);
                var systemRequest = await _eddbRepository.GetSystem(system.SystemName, lastTick).ConfigureAwait(false);

                solarSystem.ControllingFaction = systemRequest.ControllingFaction;
                solarSystem.Assets = stationRequest.Assets;
                solarSystem.States = systemRequest.States;
                solarSystem.SubFactions = systemRequest.SubFactions;
                solarSystem.UpdatedOn = stationRequest.UpdatedOn;
                faction.SolarSystems.Add(solarSystem);
            }
            return faction;
        }

        private DateTime ConvertJsonToTick(string json)
        {
            var request = EliteBgsTickRequest.FromJson(json);
            return request[0].Time.UtcDateTime;
        }

        public async Task<DateTime> GetTick()
        {
            var json = await FetchTick();
            return ConvertJsonToTick(json);
        }

        public async Task<string> FetchTick()
        {
            var url = baseUrl + "ticks";
            return await client.GetStringAsync(url).ConfigureAwait(false);
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
            if (string.IsNullOrEmpty(request.Docs[0].State))
                system.States.Add(request.Docs[0].State);
            system.Assets = await GetAssets(system.Name);
            return system;
        }
    }
}
