using System;
using System.Collections.Generic;
using System.Linq;

namespace LiteArch.Trafik.Domain
{
    public class SamplerLink : SamplerRow
    {
        public SamplerLink()
        {
        }

        public override bool IsSatisfiedBy(string row)
        {
            return !string.IsNullOrEmpty(row) && (row.StartsWith("#") || row.StartsWith($"{Constants.RedisKeyPrefix}#"));
        }

        public override SamplerRow Parse(string row)
        {
            if (string.IsNullOrEmpty(row)) return null;
            if (!IsSatisfiedBy(row)) return null;
            int index = row.IndexOf(Constants.RedisKeyPrefix);
            var data = (index < 0)
                ? row
                : row.Remove(index, Constants.RedisKeyPrefix.Length);

            data = data.TrimStart('#');
            var splits = data.Split("$");
            if (splits.Length != 2) return null;
            var source = splits[0];
            var target = splits[1];
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target)) return null;
            var sourceParts = source.Split(".");
            if (sourceParts.Length < 2) return null;
            var targetParts = target.Split(".");
            if (targetParts.Length < 2) return null;

            var result = new SamplerLink
            {
                SourceAddress = source.Substring(0, source.Length - sourceParts[sourceParts.Length - 1].Length)
                    ?.TrimEnd('.'),
                SourcePort = sourceParts[sourceParts.Length - 1],
                TargetAddress = target.Substring(0, target.Length - targetParts[targetParts.Length - 1].Length)
                    ?.TrimEnd('.'),
                TargetPort = targetParts[targetParts.Length - 1]
            };
            if (string.IsNullOrEmpty(result.SourceAddress) || string.IsNullOrEmpty(result.SourcePort) ||
                string.IsNullOrEmpty(result.TargetAddress) || string.IsNullOrEmpty(result.TargetPort))
                return null;
            return result;
        }

        public SamplerLink ReplaceDockerIps(List<SamplerIp> dockerIps)
        {
            if (dockerIps == null || !dockerIps.Any()) return this;
            Console.WriteLine($"Replacing: [{SourceAddress}, {TargetAddress}]->[{dockerIps.Count}]");
            foreach (var samplerIp in dockerIps)
            {
                Console.WriteLine($"{samplerIp.Ip}->{samplerIp.HostName}");
            }
            SourceAddress = dockerIps
                .Any(x => string.Equals(x.Ip, SourceAddress, StringComparison.CurrentCultureIgnoreCase))
                ? dockerIps
                    .FirstOrDefault(x => string.Equals(x.Ip, SourceAddress, StringComparison.CurrentCultureIgnoreCase))
                    ?.HostName
                : SourceAddress;
            TargetAddress = dockerIps
                .Any(x => string.Equals(x.Ip, TargetAddress, StringComparison.CurrentCultureIgnoreCase))
                ? dockerIps
                    .FirstOrDefault(x => string.Equals(x.Ip, TargetAddress, StringComparison.CurrentCultureIgnoreCase))
                    ?.HostName
                : TargetAddress;
            return this;
        }

        public override bool Equals(SamplerRow other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other is SamplerLink samplerLink)
            {
                return string.Equals(samplerLink.SourceAddress, SourceAddress,
                           StringComparison.CurrentCultureIgnoreCase)
                       && string.Equals(samplerLink.SourcePort, SourcePort, StringComparison.CurrentCultureIgnoreCase)
                       && string.Equals(samplerLink.TargetAddress, TargetAddress,
                           StringComparison.CurrentCultureIgnoreCase)
                       && string.Equals(samplerLink.TargetPort, TargetPort, StringComparison.CurrentCultureIgnoreCase);
            }

            return false;
        }

        public override string ToString()
        {
            return $"#{SourceAddress}.{SourcePort}${TargetAddress}.{TargetPort}";
        }

        public string TargetPort { get; set; }

        public string TargetAddress { get; set; }

        public string SourcePort { get; set; }

        public string SourceAddress { get; set; }
    }
}