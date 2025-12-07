using AutoMapper;
using RealEstateApp.Application.Dtos.Dashboard;
using RealEstateApp.WebApp.Models;

namespace RealEstateApp.WebApp.Mappings
{
    public class AdminDashboardWebProfile : Profile
    {
        public AdminDashboardWebProfile()
        {
            CreateMap<AdminDashboardDto, AdminDashboardViewModel>();
        }
    }
}

