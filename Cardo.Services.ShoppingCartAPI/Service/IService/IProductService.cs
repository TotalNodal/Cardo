using Cardo.Services.ShoppingCartAPI.Models.Dto;

namespace Cardo.Services.ShoppingCartAPI.Service.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}
