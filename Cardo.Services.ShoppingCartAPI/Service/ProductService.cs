using Cardo.Services.ShoppingCartAPI.Models.Dto;
using Cardo.Services.ShoppingCartAPI.Service.IService;
using Newtonsoft.Json;

namespace Cardo.Services.ShoppingCartAPI.Service
{
    /// <summary>
    /// Service class responsible for interacting with product-related functionalities.
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
        /// Retrieves the list of products asynchronously.
        /// </summary>
        /// <returns>The list of products if retrieval is successful; otherwise, an empty list.</returns>
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