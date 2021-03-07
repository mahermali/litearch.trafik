using System;
using Microsoft.Extensions.Configuration;

namespace LiteArch.Trafik.Consolidator
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", true, true)
                .AddEnvironmentVariables();
            var configuration = builder.Build();

            var topology = new TopologyBuilder(configuration);
            try
            {
                topology.Consolidate().Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}