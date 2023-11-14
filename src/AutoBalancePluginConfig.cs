namespace AutoBalancePlugin;
using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;

public class AutoBalancePluginConfig : BasePluginConfig
{
    [JsonPropertyName("ScrambleMode")]
    public bool ScrambleMode { get; set; } = false;
    
    [JsonPropertyName("PluginNameColor")]
    public char PluginNameColor { get; set; } = '\u0010';
    
    [JsonPropertyName("PluginMessageColor")]
    public char PluginMessageColor { get; set; } = '\u0004';
}