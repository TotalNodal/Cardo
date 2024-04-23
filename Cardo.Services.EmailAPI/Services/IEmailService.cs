using Cardo.Services.EmailAPI.Models.Dto;

namespace Cardo.Services.EmailAPI.Services
{
    public interface IEmailService
    {
        Task EmailCartAndLog(CartDto cartDto);
    }
}
