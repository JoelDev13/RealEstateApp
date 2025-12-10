using AutoMapper;
using RealEstateApp.Application.Dtos.Improvements;
using RealEstateApp.Application.Dtos.Messages;
using RealEstateApp.Application.Dtos.Offers;
using RealEstateApp.Application.Dtos.Property;
using RealEstateApp.Application.Dtos.PropertyTypes;
using RealEstateApp.Application.Dtos.SaleTypes;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Enums;

namespace RealEstateApp.Application.Mappings.EntityToDto
{
    public class DtoToEntityMappingProfile : Profile
    {
        public DtoToEntityMappingProfile()
        {
            // DTO to Entity
            CreateMap<CreatePropertyDto, Property>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsSold, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.Images, opt => opt.Ignore())
                .ForMember(dest => dest.Improvements, opt => opt.Ignore())
                .ForMember(dest => dest.PropertyType, opt => opt.Ignore())
                .ForMember(dest => dest.SaleType, opt => opt.Ignore());

            CreateMap<UpdatePropertyDto, Property>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Code, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.AgentId, opt => opt.Ignore())
                .ForMember(dest => dest.Images, opt => opt.Ignore())
                .ForMember(dest => dest.Improvements, opt => opt.Ignore())
                .ForMember(dest => dest.PropertyType, opt => opt.Ignore())
                .ForMember(dest => dest.SaleType, opt => opt.Ignore());

            // PropertyType mappings
            CreateMap<PropertyTypeDto, PropertyType>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<SaleTypeDto, SaleType>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<ImprovementDto, Improvement>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            // Offer mappings
            CreateMap<CreateOfferDto, Offer>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.OfferDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => OfferStatus.Pendiente));

            // Message mappings
            CreateMap<CreateMessageDto, Message>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SentDate, opt => opt.MapFrom(src => DateTime.UtcNow));

            // Entity to DTO (reverse mappings)
            CreateMap<PropertyType, PropertyTypeDto>();
            CreateMap<SaleType, SaleTypeDto>();
            CreateMap<Improvement, ImprovementDto>();
            
            // Property to PropertyDto
            CreateMap<Property, PropertyDto>()
                .ForMember(dest => dest.PropertyType, opt => opt.MapFrom(src => src.PropertyType != null ? src.PropertyType.Name : ""))
                .ForMember(dest => dest.SaleType, opt => opt.MapFrom(src => src.SaleType != null ? src.SaleType.Name : ""))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.MainImageUrl, opt => opt.MapFrom(src => src.Images != null && src.Images.Any() ? src.Images.First().Url : ""))
                .ForMember(dest => dest.Improvements, opt => opt.MapFrom(src => src.Improvements != null ? src.Improvements.Select(i => i.Name).ToList() : new List<string>()))
                .ForMember(dest => dest.AgentName, opt => opt.Ignore());
            
            CreateMap<Offer, OfferDto>()
                .ForMember(dest => dest.PropertyCode, opt => opt.MapFrom(src => src.Property != null ? src.Property.Code : ""))
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => "Cliente"));
            CreateMap<Message, MessageDto>()
                .ForMember(dest => dest.PropertyCode, opt => opt.MapFrom(src => src.Property != null ? src.Property.Code : ""))
                .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => "Usuario"))
                .ForMember(dest => dest.ReceiverName, opt => opt.MapFrom(src => "Usuario"))
                .ForMember(dest => dest.IsFromAgent, opt => opt.Ignore());
        }
    }
}
