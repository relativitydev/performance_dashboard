
namespace kCura.PDD.Web.Test.Services
{
    using kCura.PDD.Web.Services;
    using NUnit.Framework;

    [TestFixture]
    public class ImageProviderServiceTests
    {
        private IImageProvider _provider;
        public ImageProvider Provider
        {
            get { return _provider as ImageProvider; }
        }

        [SetUp]
        public void Initialize()
        {
            _provider = new ImageProvider();
        }

        [Test]
        public void Provider_GetImage()
        {
            //Arrange

            //Act
            var image = Provider.GetEmbeddedImageResource("down-left-green.png");

            //Assert
            Assert.IsNotNull(image);
        }
    }
}
