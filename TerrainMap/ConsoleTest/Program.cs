using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Reflection;
using TerrainMapLibrary.Interpolator.Data;
using TerrainMapLibrary.Interpolator.Kriging;
using TerrainMapLibrary.Mathematics;
using TerrainMapLibrary.Utils;
using TerrainMapLibrary.Utils.Sequence;

namespace ConsoleTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ShowTitle();
            Test();

            Console.Write("press to exit");
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
            var indicator = new CSVReader.Indicator()
            { XColumn = 0, YColumn = 1, ZColumn = 2, InvalidField = double.NaN };
            indicator.ExcludeRows.Add(0);
            var mapPoints = CSVReader.Read(filePath, indicator)
                .RemoveAll(new MapPointList.MapPoint(double.NaN, double.NaN, double.NaN));

            var counter = new StepCounter((obj) =>
            {
                if (obj.Step == obj.StepLength) { Console.SetCursorPosition(0, Console.CursorTop + 1); }

                Console.Write($"{obj.Name}   {obj.TimeLeft()}   {obj.Step.ToString("N0")}/{obj.StepLength.ToString("N0")}");
                Console.SetCursorPosition(0, Console.CursorTop);
            });

            //// build original
            //var map = SemivarianceMap.BuildOriginal(mapPoints, null, counter);
            //map.Close();

            //// build lag bins
            //var map = SemivarianceMap.Load(0, false);
            //double total = map[0].EuclidDistance + map[map.VectorCount - 1].EuclidDistance;
            //map.Close();
            //map = SemivarianceMap.Build(total / 1000, null, counter);
            //map.Close();

            //var map = SemivarianceMap.Load(SemivarianceMap.GetALlLagBins()[1]);
            //var values = new List<double>();
            //for (long i = 0; i < map.VectorCount; i++)
            //{
            //    values.Add(map[i].EuclidDistance);
            //}
            //map.Close();

            var map = SemivarianceMap
                .Load(SemivarianceMap.GetALlLagBins()[2], false);

            var image = map.GenerateImage(1366, 768);
            image.Save(@"test.png", ImageFormat.Png);

            map.Close();
        }
    }
}
