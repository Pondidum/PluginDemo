using System;
using System.IO;
using PluginDemo.Common;

namespace PluginDemo.Host
{
    internal class AssemblyManager
    {
        private AppDomain _domain;
        private Scanner _scanner;

        public void LoadFrom(string path)
        {
            if (_domain != null)
            {
                _scanner.Teardown();
                AppDomain.Unload(_domain);
            }

            var name = Path.GetFileNameWithoutExtension(path);
            var dirPath = Path.GetFullPath(Path.GetDirectoryName(path));

            var setup = new AppDomainSetup
            {
                ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
                PrivateBinPath = dirPath,
                ShadowCopyFiles = "true",
                ShadowCopyDirectories = dirPath,
            };

            _domain = AppDomain.CreateDomain(name + "Domain", AppDomain.CurrentDomain.Evidence, setup);

            var scannerType = typeof (Scanner);
            _scanner = (Scanner)_domain.CreateInstanceAndUnwrap(scannerType.Assembly.FullName, scannerType.FullName);
            _scanner.Load(name);
            _scanner.Setup();

        }

        public void DoWork()
        {
            _scanner.DoWork();
        }

        public IPlugin ShowCrossDomainPollutionExceptions()
        {
            return _scanner.ShowCrossDomainPollutionExceptions();
        }
    }
}