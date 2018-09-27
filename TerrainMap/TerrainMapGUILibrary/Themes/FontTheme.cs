using System.Drawing;

namespace TerrainMapGUILibrary.Themes
{
    internal static class FontTheme
    {
        public const string NormalString = "Arial, 13px, style=Regular";


        public static Font Normal()
        {
            var font = new Font("Arial", 13, FontStyle.Regular, GraphicsUnit.Pixel);
            return font;
        }

        public static Font NormalItalic()
        {
            var font = new Font("Arial", 13, FontStyle.Italic, GraphicsUnit.Pixel);
            return font;
        }
    }
}
