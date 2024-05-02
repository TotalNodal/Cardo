using Cardo.Services.ShoppingCartAPI.Models.Dto;
using Cardo.Services.ShoppingCartAPI.Service.IService;
using Newtonsoft.Json;

namespace Cardo.Services.ShoppingCartAPI.Service
{
    /// <summary>
    /// Service class responsible for interacting with the coupon-related functionalities.
    /// </summary>
    public class CouponService : ICouponService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="CouponService"/> class.
        /// </summary>
        /// <param name="clientFactory">The HTTP client factory.</param>
        public CouponService(IHttpClientFactory clientFactory)
        {
            _httpClientFactory = clientFactory;
        }

        /// <summary>
        /// Retrieves a coupon by its code asynchronously.
        /// </summary>
        /// <param name="couponCode">The code of the coupon to retrieve.</param>
        /// <returns>The coupon information if found; otherwise, an empty coupon.</returns>
        public async Task<CouponDto> GetCoupon(string couponCode)
        {
            var client = _httpClientFactory.CreateClient("Coupon");
            var response = await client.GetAsync($"api/coupon/GetByCode/{couponCode}");
            var apiContent = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if (resp!=null && resp.IsSuccess)
            {
                return JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(resp.Result));
            }

            return new CouponDto();
        }
    }
}