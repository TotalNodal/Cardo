namespace Cardo.Services.ProductAPI.Models.Dto
{
    /// <summary>
    /// Represents a DTO (Data Transfer Object) for a product.
    /// </summary>
    public class ProductDto
    {
        /// <summary>
        /// Gets or sets the ID of the product.
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the price of the product.
        /// </summary>
        public double Price { get; set; }
        /// <summary>
        /// Gets or sets the description of the product.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the name of the category to which the product belongs.
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// Gets or sets the URL of the product image.
        /// </summary>
        public string? ImageUrl { get; set; }
        /// <summary>
        /// Gets or sets the local path of the product image.
        /// </summary>
        public string? ImageLocalPath { get; set; }
        /// <summary>
        /// Gets or sets the image file of the product.
        /// </summary>
        public IFormFile? Image { get; set; }
    }
}
