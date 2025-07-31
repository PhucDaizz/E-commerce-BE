using Ecommerce.Infrastructure;
using ECommerce.API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext dbContext;
        private readonly IDashboardServices dashboardServices;

        public DashboardController(AppDbContext dbContext, IDashboardServices dashboardServices)
        {
            this.dbContext = dbContext;
            this.dashboardServices = dashboardServices;
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet("GetTotalRevenue")]
        public async Task<IActionResult> GetTotalRevenue()
        {
            try
            {
                var result = await dashboardServices.GetSalesRevenueAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [Authorize (Roles = "SuperAdmin, Admin")]
        [HttpGet("GetReportOrder")]
        public async Task<IActionResult> GetReportOrder()
        {
            try
            {
                var result = await dashboardServices.GetReportOrderAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [Authorize (Roles = "SuperAdmin, Admin")]
        [HttpGet("GetReportInventory")]
        public async Task<IActionResult> GetReportInventory()
        {
            try
            {
                var result = await dashboardServices.GetReportInventoryAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [Authorize (Roles = "SuperAdmin, Admin")]
        [HttpGet("GetReportUser")]
        public async Task<IActionResult> GetReportUser()
        {
            try
            {
                var result = await dashboardServices.GetReportUserAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [Authorize (Roles = "SuperAdmin, Admin")]
        [HttpGet("TopSellingProducts")]
        public async Task<IActionResult> TopSellingProducts(int items)
        {
            try
            {
                var result = await dashboardServices.TopSellingProducts(items);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [Authorize (Roles = "SuperAdmin, Admin")]
        [HttpGet("GetReportTopGender")]
        public async Task<IActionResult> GetReportTopGender()
        {
            try
            {
                var result = await dashboardServices.GetReportTopGenderAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }
}
