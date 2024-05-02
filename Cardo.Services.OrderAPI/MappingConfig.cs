using AutoMapper;
using Cardo.Services.OrderAPI.Models;
using Cardo.Services.OrderAPI.Models.Dto;

namespace Cardo.Services.OrderAPI
{
    /// <summary>
    /// Configuration class for AutoMapper mappings.
    /// </summary>
    public class MappingConfig
    {
        /// <summary>
        /// Registers mappings between DTOs and entities using AutoMapper.
        /// </summary>
        /// <returns>The MapperConfiguration instance containing the registered mappings.</returns>
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                // Mapping from OrderHeaderDto to CartHeaderDto and vice versa
                config.CreateMap<OrderHeaderDto, CartHeaderDto>()
                    .ForMember(dest => dest.CartTotal, u => u.MapFrom(src => src.OrderTotal))
                    .ReverseMap();

                // Mapping from CartDetailsDto to OrderDetailsDto
                config.CreateMap<CartDetailsDto, OrderDetailsDto>()
                    .ForMember(dest => dest.ProductName, u => u.MapFrom(src => src.Product.Name))
                    .ForMember(dest => dest.Price, u => u.MapFrom(src => src.Product.Price));

                // Reverse mapping from OrderDetailsDto to CartDetailsDto
                config.CreateMap<OrderDetailsDto, CartDetailsDto>();

                // Reverse mapping between OrderHeader and OrderHeaderDto
                config.CreateMap<OrderHeader, OrderHeaderDto>().ReverseMap();
                // Reverse mapping between OrderDetails and OrderDetailsDto
                config.CreateMap<OrderDetails, OrderDetailsDto>().ReverseMap();
            });

            return mappingConfig;
        }
    }
}
