using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Windows.Forms;

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

        public static string Ellipsis(Control owner, string text, Font font, int width)
        {
            string result = text;
            var g = owner.CreateGraphics();
            float textWidth = g.MeasureString(text, font).Width;
            if (textWidth >= width)
            {
                float testWidth = g.MeasureString("...", font).Width;
                if (testWidth >= width)
                {
                    result = string.Empty;
                }
                else
                {
                    result = "...";
                    var textParts = text.Split(new char[] { (char)32 }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < textParts.Length - 1; i++)
                    {
                        string testText = $"{string.Join(" ", textParts.Take(i + 1))} ...";
                        testWidth = g.MeasureString(testText, font).Width;
                        if (testWidth >= width)
                        {
                            break;
                        }
                        else
                        {
                            result = testText;
                        }
                    }
                }
            }

            g.Dispose();
            return result;
        }
    }
}
