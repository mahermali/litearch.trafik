namespace LiteArch.Trafik.Domain
{
    public class ConsulService
    {
        public string Name { get; set; }
        public string Node { get; set; }
        public string Address { get; set; }
        public string ServiceAddress { get; set; }
        public string DataCenter { get; set; }
        public int Port { get; set; }

        public ConsulService()
        {
            
        }
    }
}