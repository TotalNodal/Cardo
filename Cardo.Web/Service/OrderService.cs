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
    }
}
