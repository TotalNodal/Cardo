using System.Text;
using Cardo.Services.RewardAPI.Data;
using Cardo.Services.RewardAPI.Message;
using Cardo.Services.RewardAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Cardo.Services.RewardAPI.Services
{
    /// <summary>
    /// Service for managing rewards in the application.
    /// </summary>
    public class RewardService : IRewardService
    {
        private DbContextOptions<AppDbContext> _dbOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="RewardService"/> class with the specified database context options.
        /// </summary>
        /// <param name="dbOptions">The database context options.</param>
        public RewardService(DbContextOptions<AppDbContext> dbOptions)
        {
            _dbOptions = dbOptions;
        }

        /// <summary>
        /// Updates rewards based on the provided message.
        /// </summary>
        /// <param name="rewardsMessage">The rewards message containing the information to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpdateRewards(RewardsMessage rewardsMessage)
        {
            try
            {
                Rewards rewards = new()
                {
                    OrderId = rewardsMessage.OrderId,
                    RewardsActivity = rewardsMessage.RewardsActivity,
                    UserId = rewardsMessage.UserId,
                    RewardsDate = DateTime.Now
                };
                await using var _db = new AppDbContext(_dbOptions);
                _db.Rewards.AddAsync(rewards);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
            }
        }
    }
}