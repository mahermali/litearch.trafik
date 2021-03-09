using System;

namespace LiteArch.Trafik.Domain
{
    public class SamplerDockerMetadata:SamplerRow
    {
        public string Task { get; set; }
        public string HostIp { get; set; }
        public string DockerIp { get; set; }
        public override bool IsSatisfiedBy(string row)
        {
            return !string.IsNullOrEmpty(row) && row.StartsWith("$");
        }

        public override SamplerRow Parse(string row)
        {
            if (!IsSatisfiedBy(row)) return null;
            var data = row.TrimStart('$');
            var parts = data.Split(":");
            if (parts.Length != 3) return null;
            if (string.IsNullOrEmpty(parts[0]) || string.IsNullOrEmpty(parts[1])) return null;
            var result=new SamplerDockerMetadata
            {
                HostIp = parts[0],
                DockerIp=parts[1]
            };
            
            var taskSection = parts[2];
            if (!string.IsNullOrEmpty(taskSection) && taskSection.Contains("="))
            {
                var taskName = taskSection.Substring(taskSection.IndexOf("=") + 1);
                result.Task = taskName;
            }

            return result;
        }

        public override bool Equals(SamplerRow other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other is SamplerDockerMetadata samplerIp)
            {
                return string.Equals(samplerIp.DockerIp, DockerIp, StringComparison.CurrentCultureIgnoreCase) &&
                       string.Equals(samplerIp.HostIp, HostIp, StringComparison.CurrentCultureIgnoreCase) &&
                       string.Equals(samplerIp.Task, Task, StringComparison.CurrentCultureIgnoreCase);
            }

            return false;
        }

        public override string ToString()
        {
            return $"${HostIp}:{DockerIp}:{Task}";
        }
    }
}