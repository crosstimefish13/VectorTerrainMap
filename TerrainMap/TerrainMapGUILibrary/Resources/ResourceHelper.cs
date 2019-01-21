using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Reflection;

namespace TerrainMapGUILibrary.Resources
{
    internal static class ResourceHelper
    {
        public static Image GetImage(string name)
        {
            string resourcesNamespace = (typeof(ResourceHelper)).Namespace;
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream($"{resourcesNamespace}.{name}");
            var image = Image.FromStream(stream);
            stream.Dispose();
            return image;
        }

        public static Image GetArrowDownward20()
        {
            return GetImage("Icons.arrow-downward-20.png");
        }

        public static Image GetArrowUpward20()
        {
            return GetImage("Icons.arrow-upward-20.png");
        }
    }
}
