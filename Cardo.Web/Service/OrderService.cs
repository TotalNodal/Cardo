using Cardo.Web.Models;
using Cardo.Web.Service.IService;
using Cardo.Web.Utility;

namespace Cardo.Web.Service
{
    public class OrderService : IOrderService
    {
        // Fields
        private readonly IBaseService _baseService;

        // Constructor
        public OrderService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        

        public async Task<ResponseDto?> CreateOrder(CartDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data =  cartDto,
                Url = SD.OrderAPIBase + "/api/order/CreateOrder"
            });
        }

        public async Task<ResponseDto?> CreateStripeSession(StripeRequestDto stripeRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = stripeRequestDto,
                Url = SD.OrderAPIBase + "/api/order/CreateStripeSession"
            });
        }

        public async Task<ResponseDto?> GetAllOrder(string? userId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.OrderAPIBase + "/api/order/GetOrders/" + userId
            });
        }
    

        public async Task<ResponseDto?> GetOrder(int orderId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.OrderAPIBase + "/api/order/GetOrder/" + orderId
            });
        }

        public async Task<ResponseDto?> UpdateOrderStatus(int orderId, string newStatus)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = newStatus,
                Url = SD.OrderAPIBase + "/api/order/UpdateOrderStatus/"+orderId
            });
        }

        public async Task<ResponseDto?> ValidateStripeSession(int orderHeaderID)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = orderHeaderID,
                Url = SD.OrderAPIBase + "/api/order/ValidateStripeSession"
            });
        }
    }
}
