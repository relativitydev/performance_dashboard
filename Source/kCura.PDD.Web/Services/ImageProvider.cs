using System.Drawing;
using System.Reflection;

namespace kCura.PDD.Web.Services
{
    public class ImageProvider : IImageProvider
    {
        public Image GetEmbeddedImageResource(string imageName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(string.Format("{0}.Images.{1}",
                assembly.GetName().Name, imageName));
            return stream != null ? Image.FromStream(stream) : null;
        }
    }
}