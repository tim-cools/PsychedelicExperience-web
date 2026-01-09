using System.IO;
using System.Linq;
using MemBus.Support;
using PsychedelicExperience.Web.Infrastructure;
using Xunit;

namespace PsychedelicExperience.Web.Tests.Unit
{
    public class ResizeImages
    {
        [Fact(Skip = "Fix to use dynamic images and paths")]
        public void ResizeLocal()
        {
            //todo fix static folder
            const string folder = @"C:\_\temp\images";

            ImageSizes.Resize(folder, Path.Combine(folder, "200x200.fw.png"));
            ImageSizes.Resize(folder, Path.Combine(folder, "400x800.fw.png"));
            ImageSizes.Resize(folder, Path.Combine(folder, "800x400.fw.png"));

            ImageSizes.Resize(folder, Path.Combine(folder, "200x800.fw.png"));
            ImageSizes.Resize(folder, Path.Combine(folder, "800x200.fw.png"));

            ImageSizes.Resize(folder, Path.Combine(folder, "800x800.fw.png"));
        }
        
        [Fact(Skip = "Fix to use dynamic images and paths")]
        public void ResizeJeniffer()
        {
            const string folder = @"C:\_\psychedelicexperience\doc\projects\jeniffer";

            Resize(folder, "IMG_7024.jpg");
            Resize(folder, "IMG_7029.jpg");
            Resize(folder, "IMG_7030.jpg");
        }

        [Fact(Skip = "Fix to use dynamic images and paths")]
        public void ResizeToads()
        {
            var folder = @"C:\Users\Tim Cools\Pictures\201719 Toads";
            
            var files = Directory.GetFiles(folder, "P*.jpg");
            
            files
                .Select(Path.GetFileName)
                .Each(file => Resize(folder, file));
        }

        private static void Resize(string folder, string name)
        {
            ImageSizes.Resize(folder, Path.Combine(folder, name));
        }
    }
}
