using Cardo.MessageBus;
using Cardo.Services.AuthAPI.Models.Dto;
using Cardo.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cardo.Services.AuthAPI.Controllers
{
    /// <summary>
    /// Controller for user authentication and authorization.
    /// </summary>
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        protected ResponseDto _response;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthAPIController"/> class.
        /// </summary>
        /// <param name="authService">The authentication service.</param>
        /// <param name="messageBus">The message bus.</param>
        /// <param name="configuration">The configuration.</param>
        public AuthAPIController(IAuthService authService, IMessageBus messageBus, IConfiguration configuration)
        {
            _configuration = configuration;
            _messageBus = messageBus;
            _authService = authService;
            _response = new();
        }


        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="model">The registration request model.</param>
        /// <returns>An IActionResult representing the registration status.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
        {
            var errorMessage = await _authService.Register(model);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                _response.IsSuccess = false;
                _response.Message = errorMessage;
                return BadRequest(_response);
            }

            await _messageBus.PublishMessage(model.Email,
                _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue"));
            return Ok(_response);
        }

        /// <summary>
        /// Logs in a user.
        /// </summary>
        /// <param name="model">The login request model.</param>
        /// <returns>An IActionResult representing the login status.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            var loginResponse = await _authService.Login(model);
            if (loginResponse.User == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Invalid username or password";
                return BadRequest(_response);
            }
            _response.Result = loginResponse;
            return Ok(_response);
        }

        /// <summary>
        /// Assigns a role to a user.
        /// </summary>
        /// <param name="model">The registration request model containing the email and role.</param>
        /// <returns>An IActionResult representing the role assignment status.</returns>
        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDto model)
        {
            var assignRoleSuccessful = await _authService.AssignRole(model.Email, model.Role.ToUpper());
            if (!assignRoleSuccessful)
            {
                _response.IsSuccess = false;
                _response.Message = "Error encountered";
                return BadRequest(_response);
            }
            return Ok(_response);
        }
    }
}
