using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ImageProcessingApi.Services;

namespace ImageProcessingApi.Controllers
{
    [ApiController]
    [Route("api/images")]
    public class ImagesController : ControllerBase
    {
        private readonly IImageProcessingService _imageService;
        private readonly ICacheService _cacheService;

        public ImagesController(IImageProcessingService imageService, ICacheService cacheService)
        {
            _imageService = imageService;
            _cacheService = cacheService;
        }

        [HttpPost("process")]
        public async Task<IActionResult> ProcessImage([FromQuery] string filter, IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("No image file provided.");
            }

            using var memoryStream = new MemoryStream();
            await image.CopyToAsync(memoryStream);
            var imageBytes = memoryStream.ToArray();

            var cacheKey = ComputeHash(imageBytes, filter);
            var cachedImage = _cacheService.Get(cacheKey);

            if (cachedImage != null)
            {
                return File(cachedImage, "image/png");
            }

            var processedImage = _imageService.ApplyFilter(imageBytes, filter);
            _cacheService.Set(cacheKey, processedImage);

            return File(processedImage, "image/png");
        }

        private static string ComputeHash(byte[] image, string filter)
        {
            using var sha = SHA256.Create();
            var inputBytes = image.Concat(Encoding.UTF8.GetBytes(filter)).ToArray();
            return Convert.ToBase64String(sha.ComputeHash(inputBytes));
        }
    }
}