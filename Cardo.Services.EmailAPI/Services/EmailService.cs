using System.Reflection.Metadata.Ecma335;
using System.Text;
using Cardo.Services.EmailAPI.Data;
using Cardo.Services.EmailAPI.Message;
using Cardo.Services.EmailAPI.Models;
using Cardo.Services.EmailAPI.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace Cardo.Services.EmailAPI.Services
{
    /// <summary>
    /// Service for handling email-related operations.
    /// </summary>
    public class EmailService : IEmailService
    {
        private DbContextOptions<AppDbContext> _dbOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailService"/> class.
        /// </summary>
        /// <param name="dbOptions">The database options.</param>
        public EmailService(DbContextOptions<AppDbContext> dbOptions)
        {
            _dbOptions = dbOptions;
        }

        /// <summary>
        /// Sends an email containing the cart details and logs the email.
        /// </summary>
        /// <param name="cartDto">The cart DTO containing the cart details.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task EmailCartAndLog(CartDto cartDto)
        {
            StringBuilder message = new StringBuilder();

            message.AppendLine("<br/>Cart Email Requested ");
            message.AppendLine("<br/>Total " + cartDto.CartHeader.CartTotal);
            message.AppendLine("<br/>");
            message.AppendLine("<ul>");
            foreach (var item in cartDto.CartDetails)
            {
                message.AppendLine("<li>");
                message.AppendLine(item.Product.Name + " x " + item.Count);
                message.AppendLine("<li>");
            }
            message.AppendLine("</ul>");

            await LogAndEmail(message.ToString(), cartDto.CartHeader.Email);
        }

        /// <summary>
        /// Logs an order placement and sends an email.
        /// </summary>
        /// <param name="rewardsDto">The rewards DTO containing the order information.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task LogOrderPlaced(RewardsMessage rewardsDto)
        {
            string message = "New Order Placed. <br/> Order ID : " + rewardsDto.OrderId;
            await LogAndEmail(message, "josh0597@stud.kea.dk");
        }

        /// <summary>
        /// Logs a user registration and sends an email.
        /// </summary>
        /// <param name="email">The email of the registered user.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RegisterUserEmailAndLog(string email)
        {
            string message = "User Registration Successful. <br/> Email : " + email;
            await LogAndEmail(message, "josh0597@stud.kea.dk");
        }

        private async Task<bool> LogAndEmail(string message, string email)
        {
            try
            {
                EmailLogger emailLog = new()
                {
                    Email = email,
                    EmailSent = DateTime.Now,
                    Message = message
                };
                await using var _db = new AppDbContext(_dbOptions);
                _db.EmailLoggers.AddAsync(emailLog);
                await _db.SaveChangesAsync();


                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}