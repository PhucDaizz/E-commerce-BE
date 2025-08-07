using Ecommerce.Application.DTOS.Banner;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Application.Services.Impemention;
using Ecommerce.Application.Services.Interfaces;
using Ecommerce.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannerController : ControllerBase
    {
        private readonly IBannerRepository _bannerRepository;
        private readonly IBannerServices _bannerServices;

        public BannerController(IBannerRepository bannerRepository, IBannerServices bannerServices)
        {
            _bannerRepository = bannerRepository;
            _bannerServices = bannerServices;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBanners()
        {
            if (User.Identity.IsAuthenticated && (User.IsInRole("Admin") || User.IsInRole("SuperAdmin")))
            {
                var banners = await _bannerRepository.GetAllAsync(false);
                return Ok(banners);
            }
            else
            {
                var banners = await _bannerRepository.GetAllAsync(true);
                return Ok(banners);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> CreateBanner([FromForm] BannerFormModel model)
        {
            if (!ModelState.IsValid || model.ImageFile == null)
            {
                return BadRequest("Invalid input or image file is required.");
            }

            string[] allowedFileExtensions = [".jpg", ".jpeg", ".png"];
            var ext = Path.GetExtension(model.ImageFile.FileName).ToLowerInvariant();
            if (!allowedFileExtensions.Contains(ext))
            {
                return BadRequest($"Only {string.Join(", ", allowedFileExtensions)} are allowed.");
            }

            try
            {
                var command = new AddBannerImageCommand
                {
                    Banner = new Banners
                    {
                        Title = model.Title,
                        Description = model.Description,
                        RedirectUrl = model.RedirectUrl,
                        IsActive = model.IsActive,
                        DisplayOrder = model.DisplayOrder,
                        StartDate = model.StartDate,
                        EndDate = model.EndDate
                    },
                    FileName = model.ImageFile.FileName,
                    UseCloudStorage = model.UseCloudStorage
                };

                using var stream = model.ImageFile.OpenReadStream();
                var result = await _bannerServices.CreateBannerAsync(command, stream);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the banner.");
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> DeleteBanner(int id)
        {
            try
            {
                var result = await _bannerServices.DeleteBannerAsync(id);
                if (result)
                {
                    return Ok("Banner deleted successfully.");
                }
                return NotFound("Banner not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error deleting banner: {ex.Message}");
            }

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBannerById(int id)
        {
            try
            {
                if (User.Identity.IsAuthenticated && (User.IsInRole("Admin") || User.IsInRole("SuperAdmin")))
                {
                    var banner = await _bannerRepository.GetByIdAsync(id);
                    return Ok(banner);
                }
                return Unauthorized("You do not have permission to view this banner.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving banner: {ex.Message}");
            }
        }




    }
}
