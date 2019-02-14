using System.Drawing;

namespace TerrainMapGUILibrary.Resources.Images
{
    internal static class ImageHelper
    {
        public static Image GetArrowDownward(int size)
        {
            string filePath = @"Resources\Images\arrow-downward-72.png";
            var image = GetIconImage(filePath, size);
            return image;
        }

        public static Image GetArrowUpward(int size)
        {
            string filePath = @"Resources\Images\arrow-upward-72.png";
            var image = GetIconImage(filePath, size);
            return image;
        }

        private static Image GetIconImage(string filePath, int size)
        {
            Bitmap resultImage = null;
            Bitmap originalImage = null;
            Graphics graphics = null;

            try
            {
                resultImage = new Bitmap(size, size);
                originalImage = new Bitmap(filePath);
                graphics = Graphics.FromImage(resultImage);
                graphics.DrawImage(
                    originalImage,
                    new Rectangle(0, 0, resultImage.Width, resultImage.Height),
                    new Rectangle(0, 0, originalImage.Width, originalImage.Height),
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

                if (originalImage != null)
                {
                    originalImage.Dispose();
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
