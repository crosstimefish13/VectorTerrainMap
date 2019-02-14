using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace TerrainMapGUILibrary.Resources.Fonts
{
    internal static class FontHelper
    {
        private static PrivateFontCollection privateFontCollection;

        public static Font GetFont(FontStyle style = FontStyle.Regular)
        {
            Font font = null;

            try
            {
                // parse style
                bool isBold = (style & FontStyle.Bold) == FontStyle.Bold;
                bool isItalic = (style & FontStyle.Italic) == FontStyle.Italic;
                bool isUnderline = (style & FontStyle.Underline) == FontStyle.Underline;
                bool isStrikeout = (style & FontStyle.Strikeout) == FontStyle.Strikeout;

                // fontFamily
                var privateFontCollection = GetPrivateFontCollection();
                var fontFamily = privateFontCollection.Families[0];
                if (isBold == true && isItalic == true)
                {
                    fontFamily = privateFontCollection.Families[3];
                }
                else if (isBold == true)
                {
                    fontFamily = privateFontCollection.Families[1];
                }
                else if (isItalic == true)
                {
                    fontFamily = privateFontCollection.Families[2];
                }

                // fontSize
                float fontSize = Convert.ToSingle(Resource.FontSize);

                // fontStyle
                var fontStyle = FontStyle.Regular;
                if (isUnderline == true)
                {
                    fontStyle = fontStyle | FontStyle.Underline;
                }

                if (isStrikeout == true)
                {
                    fontStyle = fontStyle | FontStyle.Strikeout;
                }

                font = new Font(fontFamily, fontSize, fontStyle, GraphicsUnit.Pixel);
                return font;
            }
            catch
            {
                return font;
            }
        }

        private static PrivateFontCollection GetPrivateFontCollection()
        {
            if (privateFontCollection == null)
            {
                privateFontCollection = new PrivateFontCollection();

                // regular font
                var memory = Marshal.AllocCoTaskMem(Resource.FontRegular.Length);
                Marshal.Copy(Resource.FontRegular, 0, memory, Resource.FontRegular.Length);
                privateFontCollection.AddMemoryFont(memory, Resource.FontRegular.Length);

                // bold font
                memory = Marshal.AllocCoTaskMem(Resource.FontBold.Length);
                Marshal.Copy(Resource.FontRegular, 0, memory, Resource.FontBold.Length);
                privateFontCollection.AddMemoryFont(memory, Resource.FontBold.Length);

                // italic font
                memory = Marshal.AllocCoTaskMem(Resource.FontItalic.Length);
                Marshal.Copy(Resource.FontRegular, 0, memory, Resource.FontItalic.Length);
                privateFontCollection.AddMemoryFont(memory, Resource.FontItalic.Length);

                // bold italic font
                memory = Marshal.AllocCoTaskMem(Resource.FontBoldItalic.Length);
                Marshal.Copy(Resource.FontRegular, 0, memory, Resource.FontBoldItalic.Length);
                privateFontCollection.AddMemoryFont(memory, Resource.FontBoldItalic.Length);
            }

            return privateFontCollection;
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
