using System;
using System.Collections.Generic;
using System.Linq;

namespace LiteArch.Trafik.Domain
{
    public class SamplerLink : SamplerRow
    {
        public string TargetPort { get; set; }

        public string TargetAddress { get; set; }

        public string SourcePort { get; set; }

        public string SourceAddress { get; set; }
        public string SourceTask { get; set; }
        public string TargetTask { get; set; }

        public override bool IsSatisfiedBy(string row)
        {
            return !string.IsNullOrEmpty(row) &&
                   (row.StartsWith("#") || row.StartsWith($"{Constants.RedisKeyPrefix}#") && row.Contains("|"));
        }

        public override SamplerRow Parse(string row)
        {
            if (string.IsNullOrEmpty(row)) return null;
            if (!IsSatisfiedBy(row)) return null;
            var index = row.IndexOf(Constants.RedisKeyPrefix);
            var data = index < 0
                ? row
                : row.Remove(index, Constants.RedisKeyPrefix.Length);

            data = data.TrimStart('#');
            var sections = data.Split("|");
            if (sections.Length != 2) return null;

            //First Section before |
            var splits = sections[0].Split(":");
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

            //Second Section After |
            var secondSection = sections[1];
            if (!string.IsNullOrEmpty(secondSection))
            {
                var tasks = secondSection.Split(":");
                if (tasks.Length == 2)
                {
                    result.SourceTask = tasks[0];
                    result.TargetTask = tasks[1];
                }
            }

            if (string.IsNullOrEmpty(result.SourceAddress) || string.IsNullOrEmpty(result.SourcePort) ||
                string.IsNullOrEmpty(result.TargetAddress) || string.IsNullOrEmpty(result.TargetPort))
                return null;
            return result;
        }

        public SamplerLink ResolveDockerIPToNomadTask(List<SamplerDockerMetadata> dockerMetadatas)
        {
            if (dockerMetadatas == null || !dockerMetadatas.Any()) return this;
            SourceTask = ResolveIPToNomadTask(SourceAddress, dockerMetadatas);
            TargetTask = ResolveIPToNomadTask(TargetAddress, dockerMetadatas);
            return this;
        }

        private string ResolveIPToNomadTask(string ip, List<SamplerDockerMetadata> dockerMetadatas)
        {
            if (dockerMetadatas == null || !dockerMetadatas.Any()) return ip;
            var found = dockerMetadatas.FirstOrDefault(x =>
                string.Equals(x.DockerIp, ip, StringComparison.CurrentCultureIgnoreCase));
            return found == null ? ip : $"{found.HostIp}/{found.Task}";
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
                       && string.Equals(samplerLink.TargetPort, TargetPort, StringComparison.CurrentCultureIgnoreCase)
                       && string.Equals(samplerLink.TargetAddress, TargetAddress,
                           StringComparison.CurrentCultureIgnoreCase)
                       && string.Equals(samplerLink.SourceTask, SourceTask, StringComparison.CurrentCultureIgnoreCase);
                ;
            }

            return false;
        }

        public override string ToString()
        {
            return $"#{SourceAddress}.{SourcePort}:{TargetAddress}.{TargetPort}|{SourceTask}:{TargetTask}";
        }
    }
}