using Cardo.Services.ShoppingCartAPI.Models.Dto;

namespace Cardo.Services.ShoppingCartAPI.Service.IService
{
    public interface ICouponService
    {
        Task<CouponDto> GetCoupon(string couponCode);
    }
}
