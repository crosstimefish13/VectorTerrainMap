using System;
using System.Reflection;
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
            var matrix = new Matrix(2, 2);
            matrix[0, 0] = 0;
            matrix[0, 1] = 1;
            matrix[1, 0] = 2;
            matrix[1, 1] = 3;

            var c = matrix.Inverse();
        }
    }
}
