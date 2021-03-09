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
        private readonly int _startRetentionSeconds;
        private readonly int _maxRetentionSeconds;
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
            _startRetentionSeconds = int.Parse(configuration["Configuration:StartRetentionSeconds"]);
            _startRetentionSeconds = int.Parse(configuration["Configuration:MaxRetentionSeconds"]);
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
                    Console.WriteLine($"Docker IPs#: {_dockerIps.Count}");
                    if (!(row is SamplerLink samplerLink)) continue;
                    var key = $"{Constants.RedisKeyPrefix}{samplerLink.ReplaceDockerIps(_dockerIps)}";
                    _db.StringIncrement(key);
                    var expire = Math.Min(int.Parse(_db.StringGet(key)) * _startRetentionSeconds, _maxRetentionSeconds);
                    _db.KeyExpire(key, TimeSpan.FromSeconds(expire));
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