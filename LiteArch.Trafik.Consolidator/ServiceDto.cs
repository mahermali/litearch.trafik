namespace LiteArch.Trafik.Consolidator
{
    public class ServiceDto
    {
        public string Node { get; set; } 
        public string Address { get; set; } 
        public string Datacenter { get; set; } 
        public string ServiceAddress { get; set; }
        public int ServicePort { get; set; }
    }
}