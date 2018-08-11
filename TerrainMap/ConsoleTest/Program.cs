using System;
using System.Diagnostics;
using System.Reflection;
using TerrainMapLibrary.Interpolator.Data;
using TerrainMapLibrary.Interpolator.Kriging;
using TerrainMapLibrary.Mathematics;

namespace ConsoleTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ShowTitle();
            Test();
            Console.ReadKey(true);
        }

        private static void ShowTitle()
        {
            // display title version and copyright
            var assembly = Assembly.GetExecutingAssembly();

            var titleAttribute = assembly.GetCustomAttribute(typeof(AssemblyTitleAttribute));
            var title = (titleAttribute as AssemblyTitleAttribute).Title;
            var version = assembly.GetName().Version.ToString();
            var copyrightAttribute = assembly.GetCustomAttribute(typeof(AssemblyCopyrightAttribute));
            var copyright = (copyrightAttribute as AssemblyCopyrightAttribute).Copyright;

            Console.WriteLine($"{title} [Version {version}]");
            Console.WriteLine($"{copyright}. All rights reserved");
            Console.WriteLine();
        }

        public static void Test()
        {
            var filePath = @"..\..\..\SampleData\opendem\rostock_test\rostock.csv";
            var indicator = new CSVReader.Indicator(0, 1, 2, CSVReader.Indicator.RowMode.SkipFirstRow, double.NaN);
            var mapPoints = CSVReader.Read(filePath, indicator).RemoveInvalid(double.NaN);
            var interpolator = new KrigingInterpolator(mapPoints);

            var conuter = new KrigingInterpolator.Counter((obj) =>
            {
                Console.Write($"{obj.TimeLeft()}   {obj.Step.ToString("N0")}/{obj.StepLength.ToString("N0")}");
                Console.SetCursorPosition(0, Console.CursorTop);
            });

            interpolator.GenerateSemivarianceMapIndex(250000, 100000, conuter);
        }
    }
}
