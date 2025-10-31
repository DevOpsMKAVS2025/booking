
using Booking.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Booking.Application.Dtos;

namespace Booking.API.Controllers
{
    [Route("api/images")]
    public class ImageController : BaseApiController
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpPut("get-image-url")]
        public async Task<IActionResult> GetImageUrl([FromBody] ImageDto dto)
        {
            if (string.IsNullOrEmpty(dto.FileName))
            {
                return BadRequest("File name is required.");
            }

            var url = await _imageService.GetImageUrl(dto.FileName);
            if (url == null)
            {
                return NotFound("Image not found.");
            }

            return Ok(new { ImageUrl = url });
        }

        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage([FromForm] string filePath, [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            using (var stream = file.OpenReadStream())
            {
                await _imageService.UploadImage(filePath, stream);
            }

            return Ok("Image uploaded successfully.");
        }

        [HttpDelete("delete-image")]
        public async Task<IActionResult> DeleteImage([FromQuery] string path)
        {
            try
            {
                await _imageService.DeleteImage(path);
                return Ok(new { message = "Image deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateImage([FromForm] string oldFileName, [FromForm] string newFileName, [FromForm] IFormFile file)
        {
            try
            {
                await _imageService.DeleteImage(oldFileName);
                using (var stream = file.OpenReadStream())
                {
                    await _imageService.UploadImage(newFileName, stream);
                }

                return Ok(new { message = "Image updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
