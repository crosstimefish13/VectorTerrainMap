using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            //var interpolator = new KrigingInterpolator(mapPoints);
            //var counter = new StepCounter((obj) =>
            //{
            //    Console.Write($"{obj.TimeLeft()}   {obj.Step.ToString("N0")}/{obj.StepLength.ToString("N0")}");
            //    Console.SetCursorPosition(0, Console.CursorTop);
            //});
            //interpolator.GenerateSemivarianceMapIndex(250000, 100000, counter);
            //Console.SetCursorPosition(0, Console.CursorTop + 1);

            //interpolator.Sort(counter);
            //Console.SetCursorPosition(0, Console.CursorTop + 1);

            var counter = new StepCounter((obj) =>
            {
                if (obj.Step == obj.StepLength) { Console.SetCursorPosition(0, Console.CursorTop + 1); }

                Console.Write($"{obj.Name}   {obj.TimeLeft()}   {obj.Step.ToString("N0")}/{obj.StepLength.ToString("N0")}");
                Console.SetCursorPosition(0, Console.CursorTop);
            });

            var map = KrigingLagBinsSemivarianceMap.BuildOriginal(mapPoints, null, counter);
            map.Close();
            //var semivarianceMap = new KrigingSemivarianceMap(mapPoints);
            //semivarianceMap.BuildOriginal(counter);

            //var data = ListFileSequence.Load(@"TerrainMapLibrary");
            //var values = new List<double>();
            //for (long i = 0; i < data.Count; i++)
            //{
            //    var value = BitConverter.ToDouble(data[i], 0);
            //    values.Add(value);
            //}

            //data.Close();
        }
    }
}
