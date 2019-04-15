using System.Drawing;

namespace kCura.PDD.Web.Services
{
    public interface IImageProvider
    {
        Image GetEmbeddedImageResource(string imageName);
    }
}