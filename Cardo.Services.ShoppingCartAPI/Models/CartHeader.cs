using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cardo.Services.ShoppingCartAPI.Models
{
    /// <summary>
    /// Represents the header of a shopping cart.
    /// </summary>
    public class CartHeader
    {
        /// <summary>
        /// Gets or sets the ID of the cart header.
        /// </summary>
        [Key]
        public int CartHeaderId { get; set; }
        /// <summary>
        /// Gets or sets the ID of the user associated with this cart header.
        /// </summary>
        public string? UserId { get; set; }
        /// <summary>
        /// Gets or sets the coupon code applied to this cart header.
        /// </summary>
        public string? CouponCode { get; set; }

        /// <summary>
        /// Gets or sets the discount applied to this cart header. This property is not mapped to the database.
        /// </summary>
        [NotMapped]
        public double Discount { get; set; }
        /// <summary>
        /// Gets or sets the total amount of the cart. This property is not mapped to the database.
        /// </summary>
        [NotMapped]
        public double CartTotal { get; set; }
    }
}
