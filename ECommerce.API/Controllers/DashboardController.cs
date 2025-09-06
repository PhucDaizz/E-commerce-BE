using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Application.Services.Interfaces;
using Ecommerce.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardRepository _dashboardRepository;
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardRepository dashboardRepository, IDashboardService dashboardService)
        {
            _dashboardRepository = dashboardRepository;
            _dashboardService = dashboardService;
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

        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet("daily")]
        public async Task<IActionResult> GetDailyRevenue()
        {
            try
            {
                var result = await _dashboardService.GetDailyRevenue();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi lấy dữ liệu doanh thu ngày: {ex.Message}");
            }
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet("monthly")]
        public async Task<IActionResult> GetMonthlyRevenue()
        {
            try
            {
                var result = await _dashboardService.GetMonthlyRevenue();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi lấy dữ liệu doanh thu tháng: {ex.Message}");
            }
        }

        [HttpGet("top-locations")]
        public async Task<IActionResult> GetTopLocations([FromQuery] int topN = 10)
        {
            try
            {
                var result = await _dashboardService.GetLocationAnalysisAsync(topN);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi phân tích vị trí khách hàng: {ex.Message}");
            }
        }

        [HttpGet("province/{province}")]
        public async Task<IActionResult> GetLocationByProvince(string province)
        {
            try
            {
                var result = await _dashboardRepository.GetCustomerLocationsByProvinceAsync(province);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi lấy dữ liệu tỉnh: {ex.Message}");
            }
        }
    }
}
