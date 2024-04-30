using Cardo.Services.RewardAPI.Message;

namespace Cardo.Services.RewardAPI.Services
{
    public interface IRewardService
    {
        Task UpdateRewards(RewardsMessage rewardsMessage);
    }
}
