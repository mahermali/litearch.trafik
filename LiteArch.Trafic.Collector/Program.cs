using System;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Configuration;

namespace LiteArch.Trafic.Collector
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", true, true)
                .AddEnvironmentVariables();
            var configuration = builder.Build();
            var server = new TcpListener(IPAddress.Any, int.Parse(configuration["Configuration:Port"]));
            server.Start();
            Console.WriteLine("Starting ...");
            while (true)
            {
                new Processor(server.AcceptTcpClient(),configuration).Process().ConfigureAwait(false);
            }
        }
    }
}