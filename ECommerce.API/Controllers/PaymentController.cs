using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Application.Services.Impemention;
using Ecommerce.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VNPAY.NET;
using VNPAY.NET.Enums;
using VNPAY.NET.Models;
using VNPAY.NET.Utilities;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly string _tmnCode;
        private readonly string _hashSecret;
        private readonly string _baseUrl;
        private readonly string _callbackUrl;

        private readonly IVnpay _vnpay;
        private readonly IPaymentServices _paymentServices;
        private readonly IInventoryReservationService _inventoryReservationService;

        public PaymentController(IVnpay vnpay, IConfiguration configuration, IPaymentServices paymentServices, IInventoryReservationService inventoryReservationService)
        {
            _tmnCode = configuration["Vnpay:TmnCode"];
            _hashSecret = configuration["Vnpay:HashSecret"];
            _baseUrl = configuration["Vnpay:BaseUrl"];
            _callbackUrl = configuration["Vnpay:ReturnUrl"];
            _paymentServices = paymentServices;
            _inventoryReservationService = inventoryReservationService;
            _vnpay = vnpay;
            _vnpay.Initialize(_tmnCode, _hashSecret, _baseUrl, _callbackUrl);
        }


         /*Do Firt */
        [Authorize(Roles = "User")]
        [HttpGet("CreatePaymentUrl")]
        public async Task<ActionResult<string>> CreatePaymentUrl(double moneyToPay, string description,int? discountId)
        {
            try
            {
                var ipAddress = NetworkHelper.GetIpAddress(HttpContext); // Lấy địa chỉ IP của thiết bị thực hiện giao dịch

                var userIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized("Please login again!.");
                }
                var userId = Guid.Parse(userIdClaim.Value);

                var checkAmount =await _paymentServices.checkAmount(userId, discountId);

                if(checkAmount.IsSuccess == false)
                {
                    return BadRequest(checkAmount.Message);
                }

                var transactionId = DateTime.Now.Ticks;

                var request = new PaymentRequest
                {
                    PaymentId = transactionId,
                    Money = checkAmount.FinalAmount,
                    Description = $"{description}|{userId}|{discountId}",
                    IpAddress = ipAddress,
                    BankCode = BankCode.ANY, // Tùy chọn. Mặc định là tất cả phương thức giao dịch
                    CreatedDate = DateTime.Now, // Tùy chọn. Mặc định là thời điểm hiện tại
                    Currency = Currency.VND, // Tùy chọn. Mặc định là VND (Việt Nam đồng)
                    Language = DisplayLanguage.Vietnamese // Tùy chọn. Mặc định là tiếng Việt
                };

                var assigned = await _inventoryReservationService.AssignTransactionIdAsync(userId, transactionId.ToString());
                if (!assigned)
                {
                    return BadRequest("No active inventory reservation found. Please try again.");
                }

                var descriptionParts = request.Description.Split('|');
                
                var paymentUrl = _vnpay.GetPaymentUrl(request);         
                    
               return Created(paymentUrl, paymentUrl);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("IpnAction")]
        public async Task<IActionResult> IpnAction()
        {
            if (Request.QueryString.HasValue)
            {
                try
                {
                    var paymentResult = _vnpay.GetPaymentResult(Request.Query);
                    if (paymentResult.IsSuccess)
                    {

                        var descriptionParts = paymentResult.Description.Split('|');
                        if (descriptionParts.Length < 2)
                        {
                            return BadRequest("Invalid payment description format");
                        }

                        var description = _vnpay.GetPaymentResult(Request.Query).Description.Split('|')[0];
                        var userId = Guid.Parse(descriptionParts[1]);

                        int? discountId = null; 
                        if (!string.IsNullOrEmpty(descriptionParts[2]))
                        {
                            discountId = int.Parse(descriptionParts[2]);
                        }

                        // Lưu thông tin thanh toán vào cơ sở dữ liệu
                        var result = await _paymentServices.processPayment(paymentResult, userId, 1, discountId);

                        if (result.IsSuccess)
                        {
                            return Ok();
                        }
                        else
                        {
                            // Release reservation if payment processing failed
                            await _inventoryReservationService.ReleaseReservationAsync(userId, paymentResult.VnpayTransactionId.ToString());
                            return BadRequest(result.Message);
                        }
                    }

                    // Payment failed - need to release any reservations
                    var descriptionParts2 = paymentResult.Description.Split('|');
                    if (descriptionParts2.Length >= 2)
                    {
                        var userId = Guid.Parse(descriptionParts2[1]);
                        await _inventoryReservationService.ReleaseReservationAsync(userId, paymentResult.VnpayTransactionId.ToString());
                    }

                    return BadRequest("Thanh toán thất bại");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return NotFound("Không tìm thấy thông tin thanh toán.");
        }


        [HttpPost("PaymentCOD")]
        [Authorize]
        public async Task<IActionResult> PaymentCOD([FromBody]int? discountId)
        {
           try
           {
                var userIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized("Please login again!.");
                }
                var userId = Guid.Parse(userIdClaim.Value);

                var paymentResult = await _paymentServices.processPaymentCOD(userId, discountId);

                if (paymentResult.IsSuccess)
                {
                    return Ok(paymentResult.Message);
                }

                return BadRequest(paymentResult.Message);
           }
           catch (Exception ex)
           {
                return BadRequest(ex.Message);
           }
        }

        /*Do Second*/
        [HttpGet("Callback")]
        public ActionResult<string> Callback()
        {
            if (Request.QueryString.HasValue)
            {
                try
                {
                    var paymentResult = _vnpay.GetPaymentResult(Request.Query);
                    var resultDescription = $"{paymentResult.PaymentResponse.Description}. {paymentResult.TransactionStatus.Description}.";

                    if (paymentResult.IsSuccess)
                    {
                        return Ok(resultDescription);
                    }

                    return BadRequest(resultDescription);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return NotFound("Không tìm thấy thông tin thanh toán.");
        }
    }
}
