using Ecommerce.Application.DTOS.Payment;
using Ecommerce.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VNPAY.NET;
using VNPAY.NET.Utilities;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IVnpay _vnpay;
        private readonly IPaymentServices _paymentServices;

        public PaymentController(IVnpay vnpay, IPaymentServices paymentServices)
        {
            _paymentServices = paymentServices;
            _vnpay = vnpay;
        }


        /// <summary>
        /// Chuẩn bị thanh toán banking: tạo đơn nháp Pending + giữ hàng gắn mã giao dịch
        /// + sinh URL VNPay trong MỘT transaction. FE gọi 1 lần duy nhất rồi redirect.
        /// </summary>
        [Authorize(Roles = "User")]
        [HttpPost("PrepareBankingPayment")]
        public async Task<IActionResult> PrepareBankingPayment([FromBody] PrepareBankingPaymentRequest request)
        {
            try
            {
                var userIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized("Please login again!.");
                }
                var userId = Guid.Parse(userIdClaim.Value);
                var ipAddress = NetworkHelper.GetIpAddress(HttpContext);

                var result = await _paymentServices.PrepareBankingPaymentAsync(userId, request.Note, request.DiscountId, ipAddress);

                if (!result.IsSuccess)
                {
                    // Hết hàng sau validate: trả kèm chi tiết để FE mở modal cập nhật giỏ
                    if (result.Validation != null)
                        return BadRequest(new { message = result.Message, validation = result.Validation });
                    return BadRequest(result.Message);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// VNPay IPN (server-to-server). Gọi lại nhiều lần vẫn an toàn (idempotent).
        /// Key duy nhất là vnp_TxnRef == PaymentId lúc tạo URL.
        /// Cần đăng ký IpnUrl trỏ về endpoint này trên portal VNPay.
        /// </summary>
        [HttpGet("IpnAction")]
        public async Task<IActionResult> IpnAction()
        {
            if (Request.QueryString.HasValue)
            {
                try
                {
                    var paymentResult = _vnpay.GetPaymentResult(Request.Query);
                    // Cùng một key cho cả success lẫn fail: PaymentId == mã giao dịch đã gán lúc prepare
                    var txnRef = paymentResult.PaymentId.ToString();

                    if (paymentResult.IsSuccess)
                    {
                        var result = await _paymentServices.ConfirmBankingPaymentAsync(paymentResult);

                        if (result.IsSuccess)
                        {
                            return Ok();
                        }
                        return BadRequest(result.Message);
                    }

                    // Thanh toán thất bại: giải phóng đúng hold của txn + cancel đơn nháp
                    await _paymentServices.HandleFailedBankingPaymentAsync(txnRef);

                    return BadRequest("Thanh toán thất bại");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return NotFound("Không tìm thấy thông tin thanh toán.");
        }

        /// <summary>
        /// FE poll trạng thái đơn theo mã giao dịch sau khi redirect từ ngân hàng về.
        /// Chỉ chủ đơn mới xem được.
        /// </summary>
        [Authorize]
        [HttpGet("StatusByTxn/{txnRef}")]
        public async Task<IActionResult> GetStatusByTxn([FromRoute] string txnRef)
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("Please login again!.");
            }
            var userId = Guid.Parse(userIdClaim.Value);

            var status = await _paymentServices.GetBankingPaymentStatusAsync(txnRef, userId);
            if (status == null)
            {
                return NotFound("Transaction not found.");
            }
            return Ok(status);
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
