using System.Collections.Generic;

namespace LiteArch.Trafik.Domain
{
    public static class RowParsers
    {
        private static List<SamplerRow> _parsers = new List<SamplerRow>
        {
            new SamplerDockerMetadata(),
            new SamplerLink()
        };

        public static SamplerRow Parse(string row)
        {
            foreach (var parser in _parsers)
            {
                if (parser.IsSatisfiedBy(row)) return parser.Parse(row);
            }

            return null;
        }
    }
}