using Entities;
using Repositories.EliteBgsTypes;
using Repositories.EliteBgsTypes.FactionRequest;
using Repositories.EliteBgsTypes.StationRequest;
using Repositories.EliteBgsTypes.SystemRequest;
using Repositories.EliteBgsTypes.TickRequest;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Faction = Entities.Faction;

namespace Repositories
{
    public class EliteBgsRepository
    {
        private readonly string baseUrl = "https://elitebgs.app/api/ebgs/v4/";
        private HttpClient client = new HttpClient();
        private EddbRepository _eddbRepository;
        private FileSystemRepository _fileSystemRepository;
        private DateTime lastTick = DateTime.UtcNow;

        public EliteBgsRepository()
        {
            _eddbRepository = new EddbRepository();
            _fileSystemRepository = new FileSystemRepository();
        }

        public async Task<Faction> GetFaction(string name, bool forceUpdate = false)
        {
            lastTick = await GetTick();
            var file = "F" + name + ".log";
            var json = await _fileSystemRepository.RetrieveJsonFromFile(file).ConfigureAwait(false);
            if(string.IsNullOrEmpty(json))
                json = await FetchFaction(name).ConfigureAwait(false);
            var faction = await ConvertJsonToFaction(json).ConfigureAwait(false);
            if (faction.UpdatedOn < lastTick && !forceUpdate)
                faction = await GetFaction(name, true).ConfigureAwait(false);
            return faction;
        }

        public async Task<SolarSystem> GetSystem(string name, bool forceUpdate = false)
        {
            var file = "S" + name + ".log";
            var json = await _fileSystemRepository.RetrieveJsonFromFile(file).ConfigureAwait(false);
            if (string.IsNullOrEmpty(json) || forceUpdate)
                json = await FetchSystem(name);
            var system = await ConvertJsonToSystem(json);
            if (system.UpdatedOn < lastTick && !forceUpdate)
                system = await GetSystem(name, true).ConfigureAwait(false);
            return system;
        }

        public async Task<List<Asset>> GetStations(string systemName, bool forceUpdate = false)
        {
            var file = "A" + systemName + ".log";
            var json = await _fileSystemRepository.RetrieveJsonFromFile(file).ConfigureAwait(false);
            if (string.IsNullOrEmpty(json) || forceUpdate)
                json = await FetchStation(systemName);
            return await ConvertJsonToStations(json);
        }

        public async Task<string> FetchFaction(string name)
        {
            var file = "F" + name + ".log";
            var url = baseUrl + "factions?name=" + name;
            var response = await client.GetStringAsync(url).ConfigureAwait(false);
            await _fileSystemRepository.SaveJsonToFile(response, file);
            return response;
        }

        private async Task<Faction> ConvertJsonToFaction(string json)
        {
            var faction = new Faction();

            var request = EliteBgsFactionRequest.FromJson(json);
            faction.Name = request.Docs[0].Name;
            faction.UpdatedOn = request.Docs[0].UpdatedAt.UtcDateTime;
            
            foreach(var system in request.Docs[0].FactionPresence)
            {
                var solarSystem = new SolarSystem();
                solarSystem.Name = system.SystemName;
                solarSystem.UpdatedOn = system.UpdatedAt.UtcDateTime;
                if (system.Conflicts.Count > 0)
                {
                    solarSystem.ConflictType = system.Conflicts[0].Type;
                    solarSystem.ConflictStatus = system.Conflicts[0].Status;
                }

                var stationRequest = await GetSystem(system.SystemName).ConfigureAwait(false);
                var systemRequest = await _eddbRepository.GetSystem(system.SystemName).ConfigureAwait(false);

                solarSystem.ControllingFaction = systemRequest.ControllingFaction;
                solarSystem.Assets = stationRequest.Assets;
                solarSystem.SubFactions = systemRequest.SubFactions;
                faction.SolarSystems.Add(solarSystem);
            }
            return faction;
        }

        private async Task<SolarSystem> ConvertJsonToSystem(string json)
        {
            var system = new SolarSystem();

            var request = EliteBgsSystemRequest.FromJson(json);
            system.Name = request.Docs[0].Name;
            system.ControllingFaction = request.Docs[0].ControllingMinorFaction;
            system.UpdatedOn = request.Docs[0].UpdatedAt.UtcDateTime;
            system.Assets = await GetStations(system.Name);
            return system;
        }

        private async Task<List<Asset>> ConvertJsonToStations(string json)
        {
            var assets = new List<Asset>();

            var request = EliteBgsStationRequest.FromJson(json);
            foreach(var item in request.Docs)
            {
                var asset = new Asset();
                asset.Name = item.Name;
                asset.SolarSystem = item.System;
                asset.Faction = item.ControllingMinorFaction;
                assets.Add(asset);
            }
            return assets;
        }

        public async Task<string> FetchSystem(string name)
        {
            var file = "S" + name + ".log";
            var url = baseUrl + "systems?name=" + name;
            var response = await client.GetStringAsync(url).ConfigureAwait(false);
            await _fileSystemRepository.SaveJsonToFile(response, file);
            return response;
        }

        public async Task<string> FetchStation(string systemName)
        {
            var file = "A" + systemName + ".log";
            var url = baseUrl + "stations?system=" + systemName;
            var response = await client.GetStringAsync(url).ConfigureAwait(false);
            await _fileSystemRepository.SaveJsonToFile(response, file);
            return response;
        }

        private async Task<DateTime> ConvertJsonToTick(string json)
        {
            var request = EliteBgsTickRequest.FromJson(json);
            return request[0].Time.UtcDateTime;            
        }

        public async Task<DateTime> GetTick()
        {
            var json = await FetchTick();
            return await ConvertJsonToTick(json);
        }

        public async Task<string> FetchTick()
        {
            var url = baseUrl + "ticks";
            return await client.GetStringAsync(url).ConfigureAwait(false);
        }
    }
}
