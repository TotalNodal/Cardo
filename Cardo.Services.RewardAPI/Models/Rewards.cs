namespace Cardo.Services.RewardAPI.Models
{
    /// <summary>
    /// Represents reward information.
    /// </summary>
    public class Rewards
    {
        /// <summary>
        /// Gets or sets the unique identifier for the reward.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the user ID associated with the reward.
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// Gets or sets the date when the reward was earned.
        /// </summary>
        public DateTime RewardsDate { get; set; }
        /// <summary>
        /// Gets or sets the activity associated with the reward.
        /// </summary>
        public int RewardsActivity { get; set; }
        /// <summary>
        /// Gets or sets the order ID associated with the reward.
        /// </summary>
        public int OrderId { get; set; }

    }
}
