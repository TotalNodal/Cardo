using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cardo.Services.ShoppingCartAPI.Models.Dto;

namespace Cardo.Services.ShoppingCartAPI.Models
{
    /// <summary>
    /// Represents details of a cart item.
    /// </summary>
    public class CartDetails
    {
        /// <summary>
        /// Gets or sets the ID of the cart details.
        /// </summary>
        [Key]
        public int CartDetailsId { get; set; }
        /// <summary>
        /// Gets or sets the ID of the cart header associated with this cart details.
        /// </summary>
        public int CartHeaderId { get; set; }
        /// <summary>
        /// Gets or sets the cart header associated with this cart details.
        /// </summary>
        [ForeignKey("CartHeaderId")]
        public CartHeader CartHeader { get; set; }
        /// <summary>
        /// Gets or sets the ID of the product associated with this cart details.
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// Gets or sets the product associated with this cart details. This property is not mapped to the database.
        /// </summary>
        [NotMapped]
        public ProductDto Product { get; set; }
        /// <summary>
        /// Gets or sets the count of the product in the cart.
        /// </summary>
        public int Count { get; set; }
    }
}
