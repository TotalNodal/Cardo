using Cardo.Web.Models;

namespace Cardo.Web.Service.IService
{
    public interface IOrderService
    {
        Task<ResponseDto?> CreateOrder(CartDto cartDto);
        Task<ResponseDto?> CreateStripeSession(StripeRequestDto stripeRequestDto);
        Task<ResponseDto?> ValidateStripeSession(int orderHeaderID);
        Task<ResponseDto?> GetAllOrder(string? userId);
        Task<ResponseDto?> GetOrder(int orderId);
        Task<ResponseDto?> UpdateOrderStatus(int orderId, string status);
    }
}
