using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Dtos.Property;
using RealEstateApp.Application.Interfaces.Services;

namespace RealEstateApp.WebApp.Controllers
{
    public class AgentListController : Controller
    {
        private readonly IAgentService _agentService;
        private readonly IPropertyService _propertyService;
        private readonly IMapper _mapper;

        public AgentListController(
            IAgentService agentService,
            IPropertyService propertyService,
            IMapper mapper)
        {
            _agentService = agentService;
            _propertyService = propertyService;
            _mapper = mapper;
        }

        // GET: /AgentList
        public async Task<IActionResult> Index(string searchName)
        {
            // Obtiene todos los agentes activos
            var agents = await _agentService.GetAllActiveAgentsAsync();

            // Filtra por nombre si se proporciona
            if (!string.IsNullOrEmpty(searchName))
            {
                agents = agents.Where(a => 
                    (a.FirstName + " " + a.LastName).Contains(searchName, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Ordena alfabeticamente por nombre
            agents = agents.OrderBy(a => a.FirstName).ThenBy(a => a.LastName).ToList();

            ViewBag.SearchName = searchName;
            return View(agents);
        }

        // GET: /AgentList/Properties/agentId
        public async Task<IActionResult> Properties(string agentId, string searchCode, decimal? minPrice, decimal? maxPrice)
        {
            if (string.IsNullOrEmpty(agentId))
                return NotFound();

            // Obtiene la info del agente
            var agent = await _agentService.GetAgentByIdAsync(agentId);
            if (agent == null)
                return NotFound();

            // Obtiene las propiedades del agente
            var properties = await _propertyService.GetAgentPropertiesAsync(agentId);

            // Aplica filtros
            if (!string.IsNullOrEmpty(searchCode))
            {
                properties = properties.Where(p => p.Code.Contains(searchCode, StringComparison.OrdinalIgnoreCase));
            }

            if (minPrice.HasValue)
            {
                properties = properties.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                properties = properties.Where(p => p.Price <= maxPrice.Value);
            }

            var propertyDtos = _mapper.Map<List<PropertyDto>>(properties.ToList());

            ViewBag.Agent = agent;
            ViewBag.SearchCode = searchCode;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            return View(propertyDtos);
        }
    }
}
