using Cardo.Services.AuthAPI.Data;
using Cardo.Services.AuthAPI.Models;
using Cardo.Services.AuthAPI.Models.Dto;
using Cardo.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Identity;

namespace Cardo.Services.AuthAPI.Service
{
    /// <summary>
    /// Service for user authentication and authorization.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwTokenGenerator _jwTokenGenerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthService"/> class.
        /// </summary>
        /// <param name="db">The database context.</param>
        /// <param name="jwTokenGenerator">The JWT token generator.</param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="roleManager">The role manager.</param>
        public AuthService(AppDbContext db,
            IJwTokenGenerator jwTokenGenerator,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _jwTokenGenerator = jwTokenGenerator;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Assigns a role to a user.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <param name="roleName">The name of the role to assign.</param>
        /// <returns>True if the role was assigned successfully, otherwise false.</returns>
        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            if (user != null)
            {
                if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    //create roll if it does not exist
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Logs in a user.
        /// </summary>
        /// <param name="loginRequestDto">The login request DTO.</param>
        /// <returns>A login response DTO containing the user information and JWT token.</returns>
        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDto.UserName.ToLower());
            
            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
            
            if (user == null || isValid == false)
            {
                return new LoginResponseDto() { User = null, Token = "" };
            }

            //if user was found then Generate Jwt Token
            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwTokenGenerator.GenerateToken(user,roles);

            UserDto userDto = new()
            {
                Email = user.Email,
                ID = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber
            };

            LoginResponseDto loginResponseDto = new()
            {
                User = userDto,
                Token = token
            };

            return loginResponseDto;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="registrationRequestDto">The registration request DTO.</param>
        /// <returns>A message indicating the result of the registration process.</returns>
        public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
        {
            ApplicationUser user = new()
            {
                UserName = registrationRequestDto.Email,
                Email = registrationRequestDto.Email,
                NormalizedEmail= registrationRequestDto.Email.ToUpper(),
                Name= registrationRequestDto.Name,
                PhoneNumber= registrationRequestDto.PhoneNumber
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);
                if (result.Succeeded)
                {
                    var userToReturn = _db.ApplicationUsers.First(u=> u.UserName == registrationRequestDto.Email);

                    UserDto userDto = new()
                    {
                        Email = userToReturn.Email,
                        ID = userToReturn.Id,
                        Name = userToReturn.Name,
                        PhoneNumber = userToReturn.PhoneNumber
                    };
                    return "";
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }
            }
            catch
            {

            }
            return "Error Encountered";
        }
    }
}
