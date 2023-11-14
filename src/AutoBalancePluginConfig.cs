namespace AutoBalancePlugin;
using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;

public class AutoBalancePluginConfig : BasePluginConfig
{
    [JsonPropertyName("ScrambleMode")]
    public bool ScrambleMode { get; set; } = false;
    
    [JsonPropertyName("KillPlayerOnSwitch")]
    public bool KillPlayerOnSwitch { get; set; } = false;
    
    [JsonPropertyName("BalanceOnRoundStart")]
    public bool BalanceOnRoundStart { get; set; } = false;
    
    [JsonPropertyName("MaximumAllowedDifference")]
    public int MaximumAllowedDifference { get; set; } = 1;
    
    [JsonPropertyName("PluginNameColor")]
    public char PluginNameColor { get; set; } = '\u0010';
    
    [JsonPropertyName("PluginMessageColor")]
    public char PluginMessageColor { get; set; } = '\u0004';
}