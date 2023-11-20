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
    
    [JsonPropertyName("BalanceBots")]
    public bool BalanceBots { get; set; } = true;
    
    [JsonPropertyName("MaximumAllowedDifference")]
    public int MaximumAllowedDifference { get; set; } = 1;
    
    [JsonPropertyName("AutoBalanceMessage")]
    public string AutoBalanceMessage { get; set; } = " {GOLD}● [Auto Balance] {DEFAULT} -> {DEFAULT}Switched {GOLD}{_playerName} {DEFAULT}to {RED}{_switchedTeam}";
}