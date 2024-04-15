using AutoMapper;
using Cardo.Services.ShoppingCartAPI.Models;
using Cardo.Services.ShoppingCartAPI.Models.Dto;

namespace Cardo.Services.ShoppingCartAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CartHeader, CartHeaderDto>();
                config.CreateMap<CartHeaderDto, CartHeader>();

                config.CreateMap<CartDetails, CartDetailsDto>();
                config.CreateMap<CartDetailsDto, CartDetails>();
            });

            return mappingConfig;
        }
    }
}
