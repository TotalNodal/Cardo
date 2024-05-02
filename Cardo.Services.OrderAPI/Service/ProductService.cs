using Cardo.Services.OrderAPI.Models.Dto;
using Cardo.Services.ShoppingCartAPI.Service.IService;
using Newtonsoft.Json;

namespace Cardo.Services.ShoppingCartAPI.Service
{
    /// <summary>
    /// Service for interacting with product data.
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductService"/> class.
        /// </summary>
        /// <param name="clientFactory">The HTTP client factory.</param>
        public ProductService(IHttpClientFactory clientFactory)
        {
            _httpClientFactory = clientFactory;
        }

        /// <summary>
        /// Retrieves a list of products.
        /// </summary>
        /// <returns>The list of products.</returns>
        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            var client = _httpClientFactory.CreateClient("Product");
            var response = await client.GetAsync($"api/product");
            var apiContent = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if (resp.IsSuccess)
            {
                return JsonConvert.DeserializeObject<IEnumerable<ProductDto>>(Convert.ToString(resp.Result));
            }

            return new List<ProductDto>();
        }
    }
}