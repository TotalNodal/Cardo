using Cardo.Services.RewardAPI.Message;

namespace Cardo.Services.RewardAPI.Services
{
    /// <summary>
    /// Interface for managing rewards in the application.
    /// </summary>
    public interface IRewardService
    {
        /// <summary>
        /// Updates rewards based on the provided message.
        /// </summary>
        /// <param name="rewardsMessage">The rewards message containing the information to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateRewards(RewardsMessage rewardsMessage);
    }
}
