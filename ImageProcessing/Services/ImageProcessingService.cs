using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Png;

namespace ImageProcessingApi.Services
{
    public interface IImageProcessingService
    {
        byte[] ApplyFilter(byte[] imageData, string filter);
    }

    public class ImageProcessingService : IImageProcessingService
    {
        // Defines a method that accepts raw image bytes and a filter name, and returns the processed image as bytes.
        public byte[] ApplyFilter(byte[] imageData, string filter)
        {
            using var image = Image.Load(imageData);

            if (string.Equals(filter, "grayscale", StringComparison.OrdinalIgnoreCase))
            {
                image.Mutate(x => x.Grayscale());
            }
            else if (string.Equals(filter, "sepia", StringComparison.OrdinalIgnoreCase))
            {
                image.Mutate(x => x.Sepia());
            }

            using var memoryStream = new MemoryStream();
            image.Save(memoryStream, new PngEncoder());
            return memoryStream.ToArray();
        }

    }
}