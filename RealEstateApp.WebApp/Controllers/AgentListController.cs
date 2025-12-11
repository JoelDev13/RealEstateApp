using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Dtos.Property;
using RealEstateApp.Application.Interfaces.Services;

namespace RealEstateApp.WebApp.Controllers
{
    [AllowAnonymous]
    public class AgentListController : Controller
    {
        private readonly IAgentService _agentService;
        private readonly IPropertyService _propertyService;
        private readonly IPropertyTypeService _propertyTypeService;
        private readonly IMapper _mapper;

        public AgentListController(
            IAgentService agentService,
            IPropertyService propertyService,
            IPropertyTypeService propertyTypeService,
            IMapper mapper)
        {
            _agentService = agentService;
            _propertyService = propertyService;
            _propertyTypeService = propertyTypeService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(string searchName)
        {
            var agents = await _agentService.GetAllActiveAgentsAsync();

            if (!string.IsNullOrEmpty(searchName))
            {
                agents = agents.Where(a =>
                    (a.FirstName + " " + a.LastName)
                    .Contains(searchName, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            agents = agents.OrderBy(a => a.FirstName).ThenBy(a => a.LastName).ToList();

            ViewBag.SearchName = searchName;
            return View(agents);
        }

        public async Task<IActionResult> Properties(string agentId, [FromQuery] PropertyFiltersDto filters)
        {
            if (string.IsNullOrEmpty(agentId))
                return NotFound();

            var agent = await _agentService.GetAgentByIdAsync(agentId);
            if (agent == null)
                return NotFound();

            ViewBag.Agent = agent;

            var properties = await _propertyService.GetAvailablePropertiesByAgentAsync(agentId);
            var query = properties.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(filters.Code))
                query = query.Where(p => p.Code.Contains(filters.Code, StringComparison.OrdinalIgnoreCase));

            if (filters.PropertyTypeId.HasValue)
                query = query.Where(p => p.PropertyTypeId == filters.PropertyTypeId.Value);

            if (filters.MinPrice.HasValue)
                query = query.Where(p => p.Price >= filters.MinPrice.Value);

            if (filters.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= filters.MaxPrice.Value);

            if (filters.Bedrooms.HasValue)
                query = query.Where(p => p.Bedrooms >= filters.Bedrooms.Value);

            if (filters.Bathrooms.HasValue)
                query = query.Where(p => p.Bathrooms >= filters.Bathrooms.Value);

            query = query.OrderByDescending(p => p.CreatedAt);

            ViewBag.PropertyTypes = await _propertyTypeService.GetAllAsync();

            ViewBag.SearchCode = filters.Code;
            ViewBag.PropertyTypeId = filters.PropertyTypeId;
            ViewBag.MinPrice = filters.MinPrice;
            ViewBag.MaxPrice = filters.MaxPrice;
            ViewBag.Bedrooms = filters.Bedrooms;
            ViewBag.Bathrooms = filters.Bathrooms;

            var propertyDtos = _mapper.Map<List<PropertyDto>>(query.ToList());

            return View(propertyDtos);
        }
    }
}
