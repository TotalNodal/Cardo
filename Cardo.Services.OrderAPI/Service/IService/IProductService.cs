
using Cardo.Services.OrderAPI.Models.Dto;

namespace Cardo.Services.OrderAPI.Service.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}
