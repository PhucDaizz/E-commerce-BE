using AutoMapper;
using ECommerce.API.Models.Domain;
using ECommerce.API.Models.DTO.PaymentMethod;
using ECommerce.API.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentMethodController : ControllerBase
    {
        private readonly IPaymentMethodRepository paymentMethodRepository;
        private readonly IMapper mapper;

        public PaymentMethodController(IPaymentMethodRepository paymentMethodRepository, IMapper mapper)
        {
            this.paymentMethodRepository = paymentMethodRepository;
            this.mapper = mapper;
        }

        [HttpPost]
        [Authorize(Roles = "Admin, SuperAdmin")]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody]CreatePaymentMethodDTO createPaymentMethodDTO)
        {
            var paymentMethod = mapper.Map<PaymentMethods>(createPaymentMethodDTO);
            var result = mapper.Map<PaymentMethodDTO>(await paymentMethodRepository.AddAsync(paymentMethod));
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var paymentMethodList = await paymentMethodRepository.GetAllAsync();
            return Ok(mapper.Map<IEnumerable<PaymentMethodDTO>>(paymentMethodList));
        }
    }
}
