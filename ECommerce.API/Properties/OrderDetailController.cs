﻿using AutoMapper;
using ECommerce.API.Models.DTO.OrderDetail;
using ECommerce.API.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Properties
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IOrderDetailRepository orderDetailRepository;

        public OrderDetailController(IMapper mapper, IOrderDetailRepository orderDetailRepository)
        {
            this.mapper = mapper;
            this.orderDetailRepository = orderDetailRepository;
        }

        [HttpGet("getdetail/{orderID}")]
        [Authorize]
        public async Task<IActionResult> GetListOrderDetailsAsync([FromRoute]Guid orderID)
        {
            var orderDetails = await orderDetailRepository.GetListOrderDetailsAsync(orderID);
            if (orderDetails == null)
            {
                return NotFound("OrderId is not existing!");
            }
            return Ok(mapper.Map<IEnumerable<GetOrderDetailDTO>>(orderDetails));
        }

    }
}
