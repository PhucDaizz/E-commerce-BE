using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext dbContext;
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardController(AppDbContext dbContext, IDashboardRepository dashboardRepository)
        {
            this.dbContext = dbContext;
            _dashboardRepository = dashboardRepository;
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet("GetTotalRevenue")]
        public async Task<IActionResult> GetTotalRevenue()
        {
            try
            {
                var result = await _dashboardRepository.GetSalesRevenueAsync();
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
                var result = await _dashboardRepository.GetReportOrderAsync();
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
                var result = await _dashboardRepository.GetReportInventoryAsync();
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
                var result = await _dashboardRepository.GetReportUserAsync();
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
                var result = await _dashboardRepository.TopSellingProductsAsync(items);
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
                var result = await _dashboardRepository.GetReportTopGenderAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }
}
