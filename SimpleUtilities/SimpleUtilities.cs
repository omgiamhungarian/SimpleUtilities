using PluginAPI.Enums;
using PluginAPI.Events;
using PluginAPI.Core.Attributes;

namespace SimpleUtilities
{
    public class SimpleUtilities
    {
        public static SimpleUtilities Singleton;

        [PluginConfig]
        public Config Config;

        [PluginPriority(LoadPriority.Highest)]
        [PluginEntryPoint("SimpleUtilities", "1.0.0", "Provides simple features for your server.", "omgiamhungarian")]
        
        public void LoadPlugin()
        {
            if (!Config.IsEnabled)
            {
                return;
            }
            
            Singleton = this;
            EventManager.RegisterEvents(this);
            EventManager.RegisterEvents<EventHandlers>(this);
        }
    }
}