using AutoMapper;
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
        private readonly IMapper _mapper;

        public PaymentMethodController(IPaymentMethodRepository paymentMethodRepository, IMapper mapper)
        {
            _paymentMethodRepository = paymentMethodRepository;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize(Roles = "Admin, SuperAdmin")]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody]CreatePaymentMethodDTO createPaymentMethodDTO)
        {
            var paymentMethod = _mapper.Map<PaymentMethods>(createPaymentMethodDTO);
            var result = _mapper.Map<PaymentMethodDTO>(await _paymentMethodRepository.AddAsync(paymentMethod));
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var paymentMethodList = await _paymentMethodRepository.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<PaymentMethodDTO>>(paymentMethodList));
        }
    }
}
