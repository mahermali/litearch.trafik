using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using LiteArch.Trafik.Domain;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace LiteArch.Trafik.Collector
{
    public class Processor
    {
        private readonly TcpClient _client;
        private readonly IDatabase _db;
        private readonly int _expiresInSeconds;
        private readonly Stream _stream;
        private readonly ConnectionMultiplexer redis;
        private readonly List<SamplerIp> _dockerIps = new List<SamplerIp>();

        public Processor(TcpClient client, IConfiguration configuration)
        {
            Console.WriteLine($"Client connected: [{client.Client.RemoteEndPoint}]");
            redis = ConnectionMultiplexer.Connect(configuration["Configuration:ConnectionString"]);
            _client = client;
            _stream = _client.GetStream();
            _db = redis.GetDatabase();
            _expiresInSeconds = int.Parse(configuration["Configuration:ExpiresInSeconds"]);
        }

        public async Task Process()
        {
            var sr = new StreamReader(_stream);
            string data;
            try
            {
                while ((data = await sr.ReadLineAsync()) != null)
                {
                    Console.WriteLine(data);
                    if (string.IsNullOrEmpty(data)) continue;
                    var row = RowParsers.Parse(data);
                    if (row == null) continue;
                    Console.WriteLine(row.ToString());
                    if(row is SamplerIp samplerIp) _dockerIps.Add(samplerIp);
                    if (!(row is SamplerLink samplerLink)) continue;
                    var key = $"{Constants.RedisKeyPrefix}{samplerLink.ReplaceDockerIps(_dockerIps)}";
                    _db.StringIncrement(key);
                    _db.KeyExpire(key, TimeSpan.FromSeconds(int.Parse(_db.StringGet(key))*_expiresInSeconds));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: "+e.Message);
                _stream.Close();
            }
        }
    }
}