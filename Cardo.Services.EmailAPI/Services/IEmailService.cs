using Cardo.Services.EmailAPI.Message;
using Cardo.Services.EmailAPI.Models.Dto;

namespace Cardo.Services.EmailAPI.Services
{
    /// <summary>
    /// Interface for handling email-related operations.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends an email containing the cart details and logs the email.
        /// </summary>
        /// <param name="cartDto">The cart DTO containing the cart details.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task EmailCartAndLog(CartDto cartDto);

        /// <summary>
        /// Logs a user registration and sends an email.
        /// </summary>
        /// <param name="email">The email of the registered user.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RegisterUserEmailAndLog(string email);

        /// <summary>
        /// Logs an order placement and sends an email.
        /// </summary>
        /// <param name="rewardsDto">The rewards DTO containing the order information.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task LogOrderPlaced(RewardsMessage rewardsDto);
    }
}
