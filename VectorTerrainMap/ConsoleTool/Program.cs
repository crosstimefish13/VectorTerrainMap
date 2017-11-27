using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TerrainMapLibrary.Data;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTool
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ShowTitle();
            Kriging();
            Console.ReadKey(true);
        }

        private static void ShowTitle()
        {
            // 显示标题，版本和版权信息
            var assembly = Assembly.GetExecutingAssembly();

            var titleAttribute = assembly.GetCustomAttribute(typeof(AssemblyTitleAttribute));
            var title = (titleAttribute as AssemblyTitleAttribute).Title;

            var version = assembly.GetName().Version.ToString();

            var copyrightAttribute = assembly.GetCustomAttribute(typeof(AssemblyCopyrightAttribute));
            var copyright = (copyrightAttribute as AssemblyCopyrightAttribute).Copyright;

            Console.WriteLine($"{title} [版本 {version}]");
            Console.WriteLine($"{copyright}。保留所有权利");
            Console.WriteLine();
        }

        private static void Kriging()
        {
            var cc = new GeoNumber("-00000.23", int.MaxValue);
            var bb = new GeoNumber("1.148000000000000000000000001", int.MaxValue);
            var dd = cc + bb;

            CSVData inputData = new CSVData(@"..\..\..\SampleData\opendem\rostock\rostock.csv");
            inputData.LoadIntoMemory();
            var fields = inputData.Fields<double>();
        }
    }
}
