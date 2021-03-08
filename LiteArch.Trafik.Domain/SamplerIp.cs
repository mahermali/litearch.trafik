using System;

namespace LiteArch.Trafik.Domain
{
    public class SamplerIp:SamplerRow
    {
        public string HostName { get; set; }
        public string Ip { get; set; }
        public override bool IsSatisfiedBy(string row)
        {
            return !string.IsNullOrEmpty(row) && row.StartsWith("$");
        }

        public override SamplerRow Parse(string row)
        {
            if (!IsSatisfiedBy(row)) return null;
            var parts = row.Split(":");
            if (parts.Length != 3) return null;
            if (string.IsNullOrEmpty(parts[1]) || string.IsNullOrEmpty(parts[2])) return null;
            return new SamplerIp
            {
                HostName = parts[1],
                Ip=parts[2]
            };
        }

        public override bool Equals(SamplerRow other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other is SamplerIp samplerIp)
            {
                return string.Equals(samplerIp.Ip, Ip, StringComparison.CurrentCultureIgnoreCase) &&
                       string.Equals(samplerIp.HostName, HostName, StringComparison.CurrentCultureIgnoreCase);
            }

            return false;
        }

        public override string ToString()
        {
            return $"$:{HostName}:{Ip}";
        }
    }
}