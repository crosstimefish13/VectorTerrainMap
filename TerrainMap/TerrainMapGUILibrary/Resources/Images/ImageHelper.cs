using System.Drawing;
using TerrainMapGUILibrary.Resources;

namespace TerrainMapGUILibrary.Resources.Images
{
    internal static class ImageHelper
    {
        public static Image GetArrowDownward(int size)
        {
            var original = Resource.ImageArrowDownward96;
            var resultImage = GetImage(original, size);
            original.Dispose();

            return resultImage;
        }

        public static Image GetArrowUpward(int size)
        {
            var original = Resource.ImageArrowUpward96;
            var resultImage = GetImage(original, size);
            original.Dispose();

            return resultImage;
        }

        private static Image GetImage(Bitmap original, int size)
        {
            Bitmap resultImage = null;
            Graphics graphics = null;

            try
            {
                resultImage = new Bitmap(size, size);
                graphics = Graphics.FromImage(resultImage);
                graphics.DrawImage(
                    original,
                    new Rectangle(0, 0, resultImage.Width, resultImage.Height),
                    new Rectangle(0, 0, original.Width, original.Height),
                    GraphicsUnit.Pixel
                );
                return resultImage;
            }
            catch
            {
                if (graphics != null)
                {
                    graphics.Dispose();
                }

                if (resultImage != null)
                {
                    resultImage.Dispose();
                }

                return null;
            }
        }
    }
}
