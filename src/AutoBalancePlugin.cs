//TODO: add shuffling

using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace AutoBalancePlugin;

public class AutoBalancePlugin : BasePlugin, IPluginConfig<AutoBalancePluginConfig>
{
    public override string ModuleName => "Auto Balance Plugin";
    public override string ModuleVersion => "0.2.2";
    public override string ModuleAuthor => "hTx";
    
    public AutoBalancePluginConfig Config { get; set; } = new();

    private bool _scrambleMode;
    private bool _killPlayerOnSwitch;
    private bool _balanceOnRoundStart;
    private int _maximumAllowedDifference;
    private char _pluginNameColor;
    private char _pluginMessageColor;

    public override void Load(bool hotReload)
    {
        if(_balanceOnRoundStart)
            RegisterEventHandler<EventRoundStart>(OnRoundStart);
        else
            RegisterEventHandler<EventRoundEnd>(OnRoundEnd);

        LogToConsole(ConsoleColor.Green, $"{ModuleName} version {ModuleVersion} loaded");
    }

    public override void Unload(bool hotReload)
    {
        LogToConsole(ConsoleColor.Green, $"{ModuleName} version {ModuleVersion} unloaded");
    }
    
    public void OnConfigParsed(AutoBalancePluginConfig config)
    {
        LogToConsole(ConsoleColor.Green, $"Loading config file");
        
        this.Config = config;
        this._scrambleMode = config.ScrambleMode;
        this._killPlayerOnSwitch = config.KillPlayerOnSwitch;
        this._balanceOnRoundStart = config.BalanceOnRoundStart;
        this._maximumAllowedDifference = config.MaximumAllowedDifference;
        this._pluginNameColor = config.PluginNameColor;
        this._pluginMessageColor = config.PluginMessageColor;
    }
    
    private HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
    {
        LogToConsole(ConsoleColor.Green, $"Round ended, trying auto-balance");
        TryAutoBalance();

        return HookResult.Continue;
    }

    private HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        LogToConsole(ConsoleColor.Green, $"Round started, trying auto-balance");

        TryAutoBalance();

        return HookResult.Continue;
    }

    private bool TryAutoBalance()
    {
        var players = Utilities.GetPlayers();
        
        if (players.Count <= 0)
            return false;

        var currentlyPlaying = 
            players.FindAll(x => x.TeamNum is (int)CsTeam.CounterTerrorist or (int)CsTeam.Terrorist);
        
        if (_scrambleMode)
        {
            var shuffledPlayersList = currentlyPlaying.OrderBy(a => Guid.NewGuid()).ToList();
            for (int i = 0; i < shuffledPlayersList.Count; i++)
            {
                if(_killPlayerOnSwitch)
                    shuffledPlayersList[i].ChangeTeam(i % 2 == 0 ? CsTeam.Terrorist : CsTeam.CounterTerrorist);
                else
                    shuffledPlayersList[i].SwitchTeam(i % 2 == 0 ? CsTeam.Terrorist : CsTeam.CounterTerrorist);
            }

            return true;
        }

        var ctPlayers = players.FindAll(x => x.TeamNum == (int)CsTeam.CounterTerrorist);
        var trPlayers = players.FindAll(x => x.TeamNum == (int)CsTeam.Terrorist);

        var difference = Math.Abs(ctPlayers.Count - trPlayers.Count);

        if (difference > _maximumAllowedDifference)
        {
            var playersToSend = (int)Math.Round(difference / 2f);
            var teamWithMostPlayers = trPlayers.Count > ctPlayers.Count ? trPlayers : ctPlayers;
            var shuffledTeamPlayers = teamWithMostPlayers.OrderBy(a => Guid.NewGuid()).ToList().GetRange(0, playersToSend);
            
            if(teamWithMostPlayers == trPlayers)
                LogToConsole(ConsoleColor.Green, $"Sending {playersToSend} players to the CT Team");
            else if(teamWithMostPlayers == ctPlayers)
                LogToConsole(ConsoleColor.Green, $"Sending {playersToSend} players to the TR Team");

            foreach (var playerToSend in shuffledTeamPlayers)
            {
                var teamToSend = playerToSend.TeamNum == (int)CsTeam.Terrorist
                    ? CsTeam.CounterTerrorist
                    : CsTeam.Terrorist;
                
                if (_killPlayerOnSwitch)
                    playerToSend.ChangeTeam(teamToSend);
                else
                    playerToSend.SwitchTeam(teamToSend);

                LogToChatAll($"Switched \"{playerToSend.PlayerName}\" to {(CsTeam)playerToSend.TeamNum}");
            }
        }
        
        return true;
    }

    private void LogToConsole(string messageToLog)
    {
        Console.WriteLine($"[{ModuleName}] -> {messageToLog}");
    }
    
    private void LogToConsole(ConsoleColor color, string messageToLog)
    {
        Console.ForegroundColor = color;
        Console.WriteLine($"[{ModuleName}] -> {messageToLog}");
        Console.ResetColor();
    }
    
    private void LogToChat(CCSPlayerController? player, string messageToLog)
    {
        player?.PrintToChat($"[{ModuleName}] -> {messageToLog}");
    }
    
    private void LogToChatAll(string messageToLog)
    {
        Server.PrintToChatAll($" {_pluginNameColor}● [{ModuleName}] {ChatColors.Default} -> {_pluginMessageColor}{messageToLog}");
    }
}
