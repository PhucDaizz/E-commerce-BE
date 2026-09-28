using Ecommerce.Application.Common.Mappings;
using Ecommerce.Application.DTOS.PaymentMethod;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentMethodController : ControllerBase
    {
        private readonly IPaymentMethodRepository _paymentMethodRepository;

        public PaymentMethodController(IPaymentMethodRepository paymentMethodRepository)
        {
            _paymentMethodRepository = paymentMethodRepository;
        }

        [HttpPost]
        [Authorize(Roles = "Admin, SuperAdmin")]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody]CreatePaymentMethodDTO createPaymentMethodDTO)
        {
            var paymentMethod = createPaymentMethodDTO.ToEntity();
            var result = (await _paymentMethodRepository.AddAsync(paymentMethod)).ToPaymentMethodDTO();
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var paymentMethodList = await _paymentMethodRepository.GetAllAsync();
            return Ok(paymentMethodList.Select(x => x.ToPaymentMethodDTO()));
        }
    }
}
