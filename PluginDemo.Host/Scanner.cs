using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PluginDemo.Common;

namespace PluginDemo.Host
{
    [Serializable]
    internal class Scanner : MarshalByRefObject
    {
        private readonly List<IPlugin> _plugins;

        public Scanner()
        {
            _plugins = new List<IPlugin>();
           

        }

        private void GetAllPlugins(AppDomain domain)
        {
            var pluginType = typeof (IPlugin);

            var types = domain.GetAssemblies()
                              .SelectMany(a => a.GetTypes())
                              .Where(t => t.GetInterface(pluginType.Name) != null);

            var ctors = types.Select(t => t.GetConstructor(new Type[] {}))
                             .Where(c => c != null);

            _plugins.Clear();
            _plugins.AddRange(ctors.Select(c => c.Invoke(null))
                                   .Cast<IPlugin>());
        }

        public void Setup()
        {
            GetAllPlugins(AppDomain.CurrentDomain);
            _plugins.ForEach(p => p.Setup());
        }

        public void DoWork()
        {
            _plugins.ForEach(p => p.DoWork());
        }

        public void Teardown()
        {
            _plugins.ForEach(p => p.Teardown());
        }

        public void Load(string name)
        {
            Assembly.Load(name);
        }

        public IPlugin ShowCrossDomainPolutionExceptions()
        {
            return _plugins.First();
        }
    }
}