using AutoMapper;
using Cardo.Services.CouponAPI.Models;
using Cardo.Services.CouponAPI.Models.Dto;

namespace Cardo.Services.CouponAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<Coupon, CouponDto>();
                config.CreateMap<CouponDto, Coupon>();
            });

            return mappingConfig;
        }
    }
}
