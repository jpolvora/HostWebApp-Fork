namespace Frankstein.PluginLoader
{
    public interface IPlugin
    {
        string PluginName { get;}
        void Start();

    }
    
}