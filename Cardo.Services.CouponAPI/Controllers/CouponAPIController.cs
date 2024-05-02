using AutoMapper;
using Cardo.Services.CouponAPI.Data;
using Cardo.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Coupon = Cardo.Services.CouponAPI.Models.Coupon;

namespace Cardo.Services.CouponAPI.Controllers
{
    /// <summary>
    /// Controller for managing coupons in the web shop.
    /// </summary>
    [Route("api/coupon")]
    [ApiController]
    [Authorize]
    public class CouponAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDto _response;
        private IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="CouponAPIController"/> class.
        /// </summary>
        /// <param name="db">The database context.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        public CouponAPIController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            _response = new ResponseDto();
        }

        /// <summary>
        /// Retrieves all coupons.
        /// </summary>
        /// <returns>The response containing the list of coupons.</returns>
        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<Coupon> objList = _db.Coupons.ToList();
                _response.Result = _mapper.Map<IEnumerable<CouponDto>>(objList);
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.Message = e.Message;
            }

            return _response;
        }

        /// <summary>
        /// Retrieves a specific coupon by its ID.
        /// </summary>
        /// <param name="id">The ID of the coupon.</param>
        /// <returns>The response containing the coupon.</returns>
        [HttpGet]
        [Route("{id:int}")]
        public ResponseDto Get(int id)
        {
            try
            {
                Coupon obj = _db.Coupons.First(u=>u.CouponId==id);
                _response.Result = _mapper.Map<CouponDto>(obj);
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.Message = e.Message;
            }

            return _response;
        }

        /// <summary>
        /// Retrieves a specific coupon by its code.
        /// </summary>
        /// <param name="code">The code of the coupon.</param>
        /// <returns>The response containing the coupon.</returns>
        [HttpGet]
        [Route("GetByCode/{code}")]
        public ResponseDto GetByCode(string code)
        {
            try
            {
                Coupon obj = _db.Coupons.First(u => u.CouponCode.ToLower()== code.ToLower());
                _response.Result = _mapper.Map<CouponDto>(obj);
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.Message = e.Message;
            }

            return _response;
        }

        /// <summary>
        /// Adds a new coupon.
        /// </summary>
        /// <param name="couponDto">The coupon to add.</param>
        /// <returns>The response containing the added coupon.</returns>
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Post([FromBody] CouponDto couponDto)
        {
            try
            {
                Coupon obj = _mapper.Map<Coupon>(couponDto);
                //add coupon record to db
                _db.Coupons.Add(obj);
                //save changes to db
                _db.SaveChanges();



                
                var options = new Stripe.CouponCreateOptions
                {
                    AmountOff = (long)(couponDto.DiscountAmount*100),
                    Name = couponDto.CouponCode,
                    Currency = "dkk",
                    Id = couponDto.CouponCode,

                };
                var service = new Stripe.CouponService();
                service.Create(options);



                _response.Result = _mapper.Map<CouponDto>(obj);
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.Message = e.Message;
            }

            return _response;
        }

        /// <summary>
        /// Updates an existing coupon.
        /// </summary>
        /// <param name="couponDto">The coupon to update.</param>
        /// <returns>The response containing the updated coupon.</returns>
        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Put([FromBody] CouponDto couponDto)
        {
            try
            {
                Coupon obj = _mapper.Map<Coupon>(couponDto);
                //add coupon record to db
                _db.Coupons.Update(obj);
                //save changes to db
                _db.SaveChanges();

                _response.Result = _mapper.Map<CouponDto>(obj);
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.Message = e.Message;
            }

            return _response;
        }

        /// <summary>
        /// Deletes a coupon by its ID.
        /// </summary>
        /// <param name="id">The ID of the coupon to delete.</param>
        /// <returns>The response indicating the result of the deletion.</returns>
        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Delete(int id)
        {
            try
            {
                Coupon obj = _db.Coupons.First(u => u.CouponId == id);
                //add coupon record to db
                _db.Coupons.Remove(obj);
                //save changes to db
                _db.SaveChanges();
                //Add tombstone pattern here



                
                var service = new Stripe.CouponService();
                service.Delete(obj.CouponCode);


            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.Message = e.Message;
            }

            return _response;
        }
    }
}
