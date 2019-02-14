using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace TerrainMapGUILibrary.Resources.Fonts
{
    internal static class FontHelper
    {
        public static Font GetFont(FontStyle style = FontStyle.Regular)
        {
            // font type and styles
            bool isBold = (style & FontStyle.Bold) == FontStyle.Bold;
            bool isItalic = (style & FontStyle.Italic) == FontStyle.Italic;
            bool isUnderline = (style & FontStyle.Underline) == FontStyle.Underline;
            bool isStrikeout = (style & FontStyle.Strikeout) == FontStyle.Strikeout;

            string fontType = "Regular";
            if (isBold == true && isItalic == true)
            {
                fontType = "BoldItalic";
            }
            else if (isBold == true)
            {
                fontType = "Bold";
            }
            else if (isItalic == true)
            {
                fontType = "Italic";
            }

            var font = GetFont(fontType, isUnderline, isStrikeout);
            return font;
        }

        private static Font GetFont(string fontType, bool isUnderline, bool isStrikeout)
        {
            Font font = null;

            try
            {
                // font file path, size and style
                var fontInformation = LoadInformationFile();
                string filePath = $@"Resources\Fonts\{fontInformation[fontType]["File"]}";
                int fontSize = Convert.ToInt32(fontInformation[fontType]["Size"]);

                var fontStyle = FontStyle.Regular;
                if (isUnderline == true)
                {
                    fontStyle = fontStyle | FontStyle.Underline;
                }

                if (isStrikeout == true)
                {
                    fontStyle = fontStyle | FontStyle.Strikeout;
                }

                // create font
                var privateFontCollection = new PrivateFontCollection();
                privateFontCollection.AddFontFile(filePath);
                font = new Font(privateFontCollection.Families[0], fontSize, fontStyle, GraphicsUnit.Pixel);
                privateFontCollection.Dispose();

                return font;
            }
            catch
            {
                return font;
            }
        }

        private static Dictionary<string, Dictionary<string, string>> LoadInformationFile()
        {
            string filePath = @"Resources\Fonts\font.inf";
            var sectionRegex = new Regex(@"^\[[a-zA-Z0-9]+\]$");
            var itemRegex = new Regex(@"^[a-zA-Z0-9]+=""\S+""$");
            var result = new Dictionary<string, Dictionary<string, string>>();
            StreamReader streamReader = null;

            try
            {
                streamReader = new StreamReader(filePath);
                while (streamReader.EndOfStream == false)
                {
                    // match section
                    string section = streamReader.ReadLine();
                    var sectionMatch = sectionRegex.Match(section);
                    if (sectionMatch.Success == true)
                    {
                        string sectionName = sectionMatch.Value.Substring(1, sectionMatch.Value.Length - 2);
                        result.Add(sectionName, new Dictionary<string, string>());

                        // match each items
                        string item = streamReader.ReadLine();
                        while (string.IsNullOrEmpty(item) == false)
                        {
                            var itemMatch = itemRegex.Match(item);
                            if (itemMatch.Success == true)
                            {
                                int startIndex = 0;
                                int length = itemMatch.Value.IndexOf("=");
                                string itemName = itemMatch.Value.Substring(startIndex, length);

                                startIndex = length + 2;
                                length = itemMatch.Value.Length - startIndex - 1;
                                string itemValue = itemMatch.Value.Substring(startIndex, length);

                                result[sectionName].Add(itemName, itemValue);
                            }

                            item = streamReader.ReadLine();
                        }
                    }
                }

                return result;
            }
            catch
            {
                if (streamReader != null)
                {
                    streamReader.Dispose();
                }

                return result;
            }
        }
    }
}
