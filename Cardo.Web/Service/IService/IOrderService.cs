using Cardo.Web.Models;

namespace Cardo.Web.Service.IService
{
    public interface IOrderService
    {
        Task<ResponseDto?> CreateOrder(CartDto cartDto);

    }
}
