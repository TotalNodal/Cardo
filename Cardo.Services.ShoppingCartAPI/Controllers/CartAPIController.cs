using AutoMapper;
using Cardo.Services.ShoppingCartAPI.Data;
using Cardo.Services.ShoppingCartAPI.Models;
using Cardo.Services.ShoppingCartAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;
using Cardo.MessageBus;
using Cardo.Services.ShoppingCartAPI.RabbitMQSender;
using Cardo.Services.ShoppingCartAPI.Service.IService;

namespace Cardo.Services.ShoppingCartAPI.Controllers
{
    /// <summary>
    /// Controller for managing shopping cart operations.
    /// </summary>
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private ResponseDto _response;
        private IMapper _mapper;
        private readonly AppDbContext _db;
        private IProductService _productService;
        private ICouponService _couponService;
        private IConfiguration _configuration;
        private readonly IRabbitMQCartMessageSender _messageBus;

        /// <summary>
        /// Constructor for the CartAPIController class.
        /// </summary>
        /// <param name="mapper">An instance of AutoMapper IMapper.</param>
        /// <param name="db">The application's database context.</param>
        /// <param name="productService">The service for managing products.</param>
        /// <param name="couponService">The service for managing coupons.</param>
        /// <param name="messageBus">The message bus service for communication.</param>
        /// <param name="configuration">Represents the application's configuration.</param>
        public CartAPIController(IMapper mapper, AppDbContext db,
            IProductService productService, ICouponService couponService,
            IRabbitMQCartMessageSender messageBus, IConfiguration configuration)
        {
            _mapper = mapper;
            _messageBus = messageBus;
            _db = db;
            _productService = productService;
            _couponService = couponService;
            this._response = new ResponseDto();
            _configuration = configuration;
        }

        /// <summary>
        /// Retrieves the shopping cart for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user whose cart is being retrieved.</param>
        /// <returns>A response containing the user's shopping cart.</returns>
        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDto> GetCart(string userId)
        {
            try
            {
                CartDto cart = new()
                {
                    CartHeader = _mapper.Map<CartHeaderDto>(_db.CartHeaders.First(u => u.UserId == userId))
                };
                cart.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(_db.CartDetails
                    .Where(u=>u.CartHeaderId==cart.CartHeader.CartHeaderId));

                IEnumerable<ProductDto> productDtos = await _productService.GetProducts();

                foreach (var item in cart.CartDetails)
                {
                    item.Product = productDtos.FirstOrDefault(u => u.ProductId == item.ProductId);
                    cart.CartHeader.CartTotal += (item.Count * item.Product.Price);
                }

                //apply coupon if found and applicable
                if (!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
                {
                    CouponDto coupon = await _couponService.GetCoupon(cart.CartHeader.CouponCode);
                    if (cart.CartHeader.CouponCode != null && cart.CartHeader.CartTotal > coupon.MinAmount)
                    {
                        cart.CartHeader.CartTotal = cart.CartHeader.CartTotal - coupon.DiscountAmount;
                        cart.CartHeader.Discount=coupon.DiscountAmount;
                    }
                }

                _response.Result = cart;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        /// <summary>
        /// Applies a coupon to the user's shopping cart.
        /// </summary>
        /// <param name="cartDto">The cart DTO containing the coupon code.</param>
        /// <returns>A response indicating the success of applying the coupon.</returns>
        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var cartFromDb = await _db.CartHeaders.FirstAsync(u => u.UserId == cartDto.CartHeader.UserId);
                cartFromDb.CouponCode= cartDto.CartHeader.CouponCode;
                _db.CartHeaders.Update(cartFromDb);
                await _db.SaveChangesAsync();
                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.ToString();
            }
            return _response;
        }

        /// <summary>
        /// Sends an email for the shopping cart request.
        /// </summary>
        /// <param name="cartDto">The cart DTO containing the email information.</param>
        /// <returns>A response indicating the success of sending the email.</returns>
        [HttpPost("EmailCartRequest")]
        public async Task<object> EmailCartRequest([FromBody] CartDto cartDto)
        {
            try
            {
                _messageBus.SendMessage(cartDto, _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue"));
                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.ToString();
            }
            return _response;
        }


        /// <summary>
        /// Upserts (update/insert) the user's shopping cart.
        /// </summary>
        /// <param name="cartDto">The cart DTO containing the cart information to upsert.</param>
        /// <returns>A response indicating the success of the upsert operation.</returns>
        [HttpPost("CartUpsert")]
        public async Task<ResponseDto> CartUpsert(CartDto cartDto)
        {
            try
            {
                var cartHeaderFromDb = await _db.CartHeaders.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId);
                if (cartHeaderFromDb == null)
                {
                    //creates header and details
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
                    _db.CartHeaders.Add(cartHeader);
                    await _db.SaveChangesAsync();
                    cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                    _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                    await _db.SaveChangesAsync();
                }
                else
                {
                    //if header exists then we will update the cart details
                    var cartDetailsFromDb = await _db.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                        u => u.ProductId == cartDto.CartDetails.First().ProductId &&
                             u.CartHeaderId == cartHeaderFromDb.CartHeaderId);
                    if (cartDetailsFromDb == null)
                    {
                        //create details
                        cartDto.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                        _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        //update count in details
                        cartDto.CartDetails.First().Count += cartDetailsFromDb.Count;
                        cartDto.CartDetails.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
                        cartDto.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;
                        _db.CartDetails.Update(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _db.SaveChangesAsync();
                    }
                }

                _response.Result = cartDto;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }

        /// <summary>
        /// Removes an item from the shopping cart.
        /// </summary>
        /// <param name="cartDetailsId">The ID of the cart details to remove.</param>
        /// <returns>A response indicating the success of removing the item from the cart.</returns>
        [HttpPost("RemoveCart")]
        public async Task<ResponseDto> RemoveCart([FromBody]int cartDetailsId)
        {
            try
            {
                CartDetails cartDetails = _db.CartDetails.
                    First(u => u.CartDetailsId == cartDetailsId);

                int totalCountofCartItem= _db.CartDetails.Where(u => u.CartHeaderId == cartDetails.CartHeaderId).Count();
                _db.CartDetails.Remove(cartDetails);
                if (totalCountofCartItem == 1)
                {
                    var cartHeaderToRemove = await _db.CartHeaders
                        .FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);
                    _db.CartHeaders.Remove(cartHeaderToRemove);
                    await _db.SaveChangesAsync();
                }
                await _db.SaveChangesAsync();

                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }
    }
}
