using System;
using System.Reflection;

namespace ConsoleTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ShowTitle();
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
    }
}
