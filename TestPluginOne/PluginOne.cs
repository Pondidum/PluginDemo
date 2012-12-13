using System;
using PluginDemo.Common;

namespace TestPluginOne
{
    public class PluginOne : IPlugin
    {
        public void Setup()
        {
            Console.WriteLine("PluginOne.Setup");
        }

        public void DoWork()
        {
            Console.WriteLine("PluginOne.DoWork");
        }

        public void Teardown()
        {
            Console.WriteLine("PluginOne.Teardown");
        }
    }
}
