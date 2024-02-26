using Microsoft.AspNetCore.Identity;

namespace Cardo.Services.AuthAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
