using Cardo.Services.AuthAPI.Models;

namespace Cardo.Services.AuthAPI.Service.IService
{
    public interface IJwTokenGenerator
    {
        string GenerateToken(ApplicationUser applicationUser);
    }
}
