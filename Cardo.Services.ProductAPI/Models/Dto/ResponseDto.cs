namespace Cardo.Services.ProductAPI.Models.Dto
{
    /// <summary>
    /// Represents a DTO (Data Transfer Object) for the response.
    /// </summary>
    public class ResponseDto
    {
        /// <summary>
        /// Gets or sets the result of the response.
        /// </summary>
        public object? Result { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the operation was successful.
        /// </summary>
        public bool IsSuccess { get; set; } = true;
        /// <summary>
        /// Gets or sets a message describing the response.
        /// </summary>
        public string Message { get; set; } = "";

    }
}
