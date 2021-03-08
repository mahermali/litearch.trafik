using LiteArch.Trafik.Api.Services;
using LiteArch.Trafik.Domain;
using Microsoft.AspNetCore.Mvc;

namespace LiteArch.Trafik.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TopologyController:ControllerBase
    {
        private readonly ITopologyService _topologyService;

        public TopologyController(ITopologyService topologyService)
        {
            _topologyService = topologyService;
        }

        [HttpGet]
        public Topology Get()
        {
            return _topologyService.Get();
        }
    }
}