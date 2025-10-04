using Dalamud.Configuration;
using System;

namespace FFXIVQuotesClient;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;
    public string BaseUrl { get; set; } = "";
    public string ApiToken { get; set; } = "";
    
    
    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}
