using System;
using System.IO;

namespace PluginDemo.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            Directory.CreateDirectory("plugins");

            var manager = new AssemblyManager();

            manager.LoadFrom("plugins\\TestPluginOne.dll");
            manager.DoWork();

           Directory.Delete("plugins", true);

            manager.DoWork();
            manager.ShowCrossDomainPolutionExceptions();

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
