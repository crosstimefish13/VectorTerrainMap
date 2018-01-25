using System.IO;
using System.Reflection;

namespace TerrainMapLibrary.Data
{
    internal class EmbeddedCSVData : CSVData
    {
        public EmbeddedCSVData(string name)
            : base(name)
        { }

        protected override Stream GetStream()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(FilePath);
            return stream;
        }
    }
}
