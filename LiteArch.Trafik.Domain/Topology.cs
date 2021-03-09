using System.Collections.Generic;
using System.Linq;

namespace LiteArch.Trafik.Domain
{
    public class Topology
    {
        public Topology()
        {
            Nodes = new List<TopologyNode>();
            Links = new List<TopologyLink>();
        }
        public List<TopologyLink> Links { get; set; }
        public List<TopologyNode> Nodes { get; set; }

        public void BuildNodesFromLinks()
        {
            Nodes.AddRange(Links.Select(x => x.Source).Union(Links.Select(x => x.Target)).Distinct().Select(x => new TopologyNode
            {
                Id = x
            }).ToList());
        }
    }
}