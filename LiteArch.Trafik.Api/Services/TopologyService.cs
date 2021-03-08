using System.Text.Json;
using LiteArch.Trafik.Domain;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace LiteArch.Trafik.Api.Services
{
    public class TopologyService : ITopologyService
    {
        private readonly IConfiguration _configuration;
        private readonly IDatabase _database;

        public TopologyService(IConnectionMultiplexer connectionMultiplexer)
        {
            _database = connectionMultiplexer.GetDatabase();
        }

        public Topology Get()
        {
            var json = _database.StringGet(Constants.TopologyKey);
            return JsonSerializer.Deserialize<Topology>(json);
        }
    }
}