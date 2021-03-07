namespace LiteArch.Trafik.Domain
{
    public class TopologyLink
    {
        public int Id { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }
        public int Weight { get;  set;}
    }
}