namespace PluginDemo.Common
{
    public interface IPlugin
    {
        void Setup();
        void DoWork();
        void Teardown();
    }
}
