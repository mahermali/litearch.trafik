using System;

namespace LiteArch.Trafik.Domain
{
    public abstract class SamplerRow:IEquatable<SamplerRow>
    {
        public abstract bool IsSatisfiedBy(string row);
        public abstract SamplerRow Parse(string row);
        public abstract bool Equals(SamplerRow other);
        public abstract override string ToString();
    }
}