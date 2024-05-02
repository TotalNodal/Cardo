
using Cardo.Services.OrderAPI.Models.Dto;

namespace Cardo.Services.ShoppingCartAPI.Service.IService
{
    /// <summary>
    /// Interface for interacting with product data.
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Retrieves a list of products.
        /// </summary>
        /// <returns>The list of products.</returns>
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}
