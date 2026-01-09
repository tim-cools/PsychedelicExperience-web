using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Marten.Schema.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PsychedelicExperience.Common;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using static System.IO.File;

namespace PsychedelicExperience.Web.Infrastructure
{
    internal static class FormFileExtensions
    {
        internal static async Task<IEnumerable<FormFile>> Save(this ICollection<IFormFile> files,
            IConfiguration configuration)
        {
            var folder = configuration.PhotosFolder();

            var result = new List<FormFile>();
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var formFile = await file.Save(folder);
                    ImageSizes.Resize(folder, formFile.FileName);
                    result.Add(formFile);
                }
            }
            return result;
        }

        internal static async Task<FormFile> Save(this IFormFile file, IConfiguration configuration)
        {
            var folder = configuration.PhotosFolder();
            var result = await file.Save(folder);
            ImageSizes.Resize(folder, result.FileName);
            return result;
        }

        internal static async Task<FormFile> Save(this IFormFile file, string fullFolder)
        {
            var id = new ShortGuid(CombGuidIdGeneration.NewGuid());
            var fileName = Path.ChangeExtension(Path.Combine(fullFolder, id), Path.GetExtension(file.FileName));

            using (var fileStream = new FileStream(fileName, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
                return new FormFile(id, fileName, file.FileName);
            }
        }
    }

    public class ImageSize
    {
        public int Width { get; }
        public int Height { get; }

        public ImageSize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public string FileName(string folder, string fileName) => Path.Combine(folder, $"h{Height}w{Width}-{fileName}");

        public bool Is(int height, int width) => Height == height && Width == width;
    }

    public static class ImageSizes
    {
        public static ImageSize W32H32 { get; } = new ImageSize(32, 32);
        public static ImageSize S60 { get; } = new ImageSize(60, 60);
        public static ImageSize S150 { get; } = new ImageSize(150, 150);
        public static ImageSize W180H12 { get; } = new ImageSize(180, 120);
        public static ImageSize W750H400 { get; } = new ImageSize(750, 400);

        public static void Resize(string folder, string resultFileName)
        {
            Resize(folder, resultFileName, W32H32, true);
            Resize(folder, resultFileName, S60, true);
            Resize(folder, resultFileName, S150, true);
            Resize(folder, resultFileName, W180H12, true);
            Resize(folder, resultFileName, W750H400, false);
        }

        private static void Resize(string folder, string fileName, ImageSize imageSize, bool cropAlways)
        {
            if (imageSize == null) throw new ArgumentNullException(nameof(imageSize));

            var newFileName = imageSize.FileName(folder, Path.GetFileName(fileName));

            using var image = Image.Load(fileName);
            using var resized = ScaleImage(image, imageSize, cropAlways);

            resized.Save(newFileName);
        }

        private static Image<Rgba32> ScaleImage(Image<Rgba32> image, ImageSize imageSize, bool cropAlways)
        {
            var crop = cropAlways || ShouldCrop(image, imageSize);

            var options = new ResizeOptions
            {
                Size = new Size(imageSize.Width, imageSize.Height),
                Mode = crop ? ResizeMode.Crop : ResizeMode.BoxPad,
                Position = AnchorPositionMode.Center
            };
            
            image.Mutate(mutate => mutate
                .Resize(options)
                .BackgroundColor(Rgba32.White));

            return image;
        }

        private static bool ShouldCrop(Image<Rgba32> image, ImageSize imageSize)
        {
            var scaling = image.Height < image.Width
                ? (float) image.Height / image.Width
                : (float) image.Width / image.Height;

            var targetScaling = imageSize.Height < imageSize.Width
                ? (float) imageSize.Height / imageSize.Width
                : (float) imageSize.Width / imageSize.Height;

            const double threshold = 0.25;
            var scaleRatio = targetScaling / scaling;
            var crop = scaleRatio >= 1 - threshold && scaleRatio <= 1 + threshold;
            return crop;
        }

        private static Image<Rgba32> CenterSmallImage(Image<Rgba32> image, ImageSize imageSize)
        {
            var newImage = new Image<Rgba32>(imageSize.Width, imageSize.Height);
            var location = new Point((imageSize.Width - image.Width) / 2, (imageSize.Height - image.Height) / 2);
            
            newImage.Mutate(mutate => mutate
                .Fill(Rgba32.White)
                .DrawImage(image, location, 1F));
            
            return newImage;
        }

        public static ImageSize Get(int height, int width)
        {
            if (W32H32.Is(height, width)) return W32H32;
            if (W180H12.Is(height, width)) return W180H12;
            if (W750H400.Is(height, width)) return W750H400;

            return null;
        }

        public static string GetResized(string folder, string fileName, int height, int width)
        {
            var size = Get(height, width);
            if (size == null) return null;

            var resized = size.FileName(folder, Path.GetFileName(fileName));
            if (!Exists(resized))
            {
                Resize(folder, fileName, size, false);
            }
            return resized;
        }
    }
}