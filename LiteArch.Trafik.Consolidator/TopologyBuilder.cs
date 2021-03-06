using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Authentication;
using System.Text.Json;
using System.Threading.Tasks;
using LiteArch.Trafik.Domain;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

namespace LiteArch.Trafik.Consolidator
{
    public class TopologyBuilder : IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _consulClient;
        private readonly ConnectionMultiplexer _redisConnection;
        private readonly IDatabase _redisDatabase;

        public TopologyBuilder(IConfiguration configuration)
        {
            _configuration = configuration;
            var handler = new HttpClientHandler
            {
                SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls,
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            _consulClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(configuration["Configuration:ConsulUrl"])
            };
            if (!string.IsNullOrEmpty(configuration["Configuration:ConsulToken"]))
                _consulClient.DefaultRequestHeaders.Add("X-CONSUL-Token", configuration["Configuration:Token"]);
            _redisConnection = ConnectionMultiplexer.Connect(configuration["Configuration:ConnectionString"]);
            _redisDatabase = _redisConnection.GetDatabase();
        }

        public List<ConsulService> Services { get; set; }

        public void Dispose()
        {
            _consulClient?.Dispose();
            _redisConnection?.Dispose();
        }

        public async Task Consolidate()
        {
            await BuildConsulServices();
            await RetrieveLinksAndCrossReferenceWithConsul();
        }

        private async Task BuildConsulServices()
        {
            var servicesResponse = await _consulClient.GetAsync("/v1/catalog/services");
            if (!servicesResponse.IsSuccessStatusCode) return;
            var services = (await servicesResponse.Content.ReadAsAsync<JObject>()).Properties().Select(x => x.Name)
                .ToList();
            Services = new List<ConsulService>();
            foreach (var s in services)
            {
                var serviceResponse = await _consulClient.GetAsync($"/v1/catalog/service/{s}");
                if (!serviceResponse.IsSuccessStatusCode) continue;
                var service = await serviceResponse.Content.ReadAsAsync<ServiceDto[]>();
                foreach (var serviceInstance in service)
                    Services.Add(new ConsulService
                    {
                        Address = serviceInstance.Address,
                        Port = serviceInstance.ServicePort,
                        Name = s,
                        Node = serviceInstance.Node,
                        ServiceAddress = serviceInstance.ServiceAddress,
                        DataCenter = serviceInstance.Datacenter
                    });
            }
        }

        private async Task RetrieveLinksAndCrossReferenceWithConsul()
        {
            var topology = new Topology();

            var server = _redisConnection.GetServer(_configuration["Configuration:ConnectionString"]);
            var samplerLink = new SamplerLink();
            var lastIndex = 1;
            foreach (var key in server.Keys(pattern: $"{Constants.RedisKeyPrefix}*"))
            {
                var link = samplerLink.Parse(key) as SamplerLink;
                if (link == null) continue;
                var weightValue = _redisDatabase.StringGet(key);
                var weight = string.IsNullOrEmpty(weightValue) ? 0 : int.Parse(weightValue);
                topology.Links.Add(new TopologyLink
                {
                    Id = ++lastIndex,
                    Source = string.IsNullOrEmpty(link.SourceTask) ? ResolveServiceName($"{link.SourceAddress}:{link.SourcePort}")
                        : link.SourceTask,
                    Target = string.IsNullOrEmpty(link.TargetTask) ? ResolveServiceName($"{link.TargetAddress}:{link.TargetPort}")
                        : link.TargetTask,
                    Weight = weight
                });
            }

            topology.BuildNodesFromLinks();

            _redisDatabase.StringSet(Constants.TopologyKey, JsonSerializer.Serialize(topology));
        }

        private string ResolveServiceName(string addressAndPort)
        {
            foreach (var service in Services)
                if (string.Equals($"{service.Address}:{service.Port}", addressAndPort,
                    StringComparison.CurrentCultureIgnoreCase))
                    return $"{service.Address}/{service.Name}";

            return addressAndPort;
        }
    }
}