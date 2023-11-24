using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Utils;

namespace AutoBalancePlugin;

public class AutoBalancePlugin : BasePlugin, IPluginConfig<AutoBalancePluginConfig>
{
    public override string ModuleName => "Auto Balance Plugin";
    public override string ModuleVersion => "0.4.2";
    public override string ModuleAuthor => "hTx";
    
    public AutoBalancePluginConfig Config { get; set; } = new();

    private bool _scrambleMode;
    private bool _killPlayerOnSwitch;
    private bool _balanceOnRoundStart;
    private bool _balanceBots;
    private int _maximumAllowedDifference;
    private string _autoBalanceMessage = "";

    public override void Load(bool hotReload)
    {
        LogHelper.LogToConsole(ConsoleColor.Green, $"[Auto Balance Plugin] -> {ModuleName} version {ModuleVersion} loaded");
    }

    public override void Unload(bool hotReload)
    {
        LogHelper.LogToConsole(ConsoleColor.Green, $"[Auto Balance Plugin] -> {ModuleName} version {ModuleVersion} unloaded");
    }

    [GameEventHandler(HookMode.Post)]
    public HookResult OnPlayerConnect(EventPlayerConnect @event, GameEventInfo info)
    {
        LogHelper.LogToConsole(ConsoleColor.Green, "Player Connect test");
        
        return HookResult.Continue;
    }
    
    [GameEventHandler(HookMode.Post)]
    public HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
    {
        if(_balanceOnRoundStart)
            return HookResult.Continue;
            
        LogHelper.LogToConsole(ConsoleColor.Green, $"[Auto Balance Plugin] -> Round ended, trying auto-balance");
        
        TryAutoBalance();
        
        return HookResult.Continue;
    }
    
    [GameEventHandler(HookMode.Post)]
    public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        if(!_balanceOnRoundStart)
            return HookResult.Continue;
        
        LogHelper.LogToConsole(ConsoleColor.Green, $"[Auto Balance Plugin] -> Round started, trying auto-balance");
        
        TryAutoBalance();
        
        return HookResult.Continue;
    }
    
    public void OnConfigParsed(AutoBalancePluginConfig config)
    {
        LogHelper.LogToConsole(ConsoleColor.Green, $"[Auto Balance Plugin] -> Loading config file");
        
        this.Config = config;
        this._scrambleMode = config.ScrambleMode;
        this._killPlayerOnSwitch = config.KillPlayerOnSwitch;
        this._balanceOnRoundStart = config.BalanceOnRoundStart;
        this._balanceBots = config.BalanceBots;
        this._maximumAllowedDifference = config.MaximumAllowedDifference;
        this._autoBalanceMessage = config.AutoBalanceMessage;
    }

    private void TryAutoBalance()
    {
        var players = Utilities.GetPlayers();
        
        if (players.Count <= 0)
            return;

        var currentlyPlaying = _balanceBots 
            ? players.FindAll(x => x.TeamNum is (int)CsTeam.CounterTerrorist or (int)CsTeam.Terrorist)
            : players.FindAll(x => (x.TeamNum is (int)CsTeam.CounterTerrorist or (int)CsTeam.Terrorist) && !x.IsBot);
        
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

            return;
        }

        var ctPlayers = currentlyPlaying.FindAll(x => x.TeamNum == (int)CsTeam.CounterTerrorist);
        var trPlayers = currentlyPlaying.FindAll(x => x.TeamNum == (int)CsTeam.Terrorist);

        var difference = Math.Abs(ctPlayers.Count - trPlayers.Count);

        if (difference <= _maximumAllowedDifference)
            return;

        var playersToSend = (int)Math.Round(difference / 2f);
        var teamWithMostPlayers = trPlayers.Count > ctPlayers.Count ? trPlayers : ctPlayers;
        var shuffledTeamPlayers = teamWithMostPlayers.OrderBy(a => Guid.NewGuid()).ToList().GetRange(0, playersToSend);
        
        if(teamWithMostPlayers == trPlayers)
            LogHelper.LogToConsole(ConsoleColor.Green, $"[Auto Balance Plugin] -> Sending {playersToSend} players to the CT Team");
        else if(teamWithMostPlayers == ctPlayers)
            LogHelper.LogToConsole(ConsoleColor.Green, $"[Auto Balance Plugin] -> Sending {playersToSend} players to the TR Team");

        foreach (var playerToSend in shuffledTeamPlayers)
        {
            var teamToSend = playerToSend.TeamNum == (int)CsTeam.Terrorist
                ? CsTeam.CounterTerrorist
                : CsTeam.Terrorist;
            
            var teamAbbreviation = (CsTeam)playerToSend.TeamNum == CsTeam.Terrorist ? "T" :
                (CsTeam)playerToSend.TeamNum == CsTeam.CounterTerrorist ? "CT"
                : "Unknown";
            
            if (_killPlayerOnSwitch)
                playerToSend.ChangeTeam(teamToSend);
            else
                playerToSend.SwitchTeam(teamToSend);

            var tempAutoBalanceMessage = _autoBalanceMessage.Replace("{_playerName}", playerToSend.PlayerName);
            tempAutoBalanceMessage = tempAutoBalanceMessage.Replace("{_switchedTeam}", teamAbbreviation);

            
            LogHelper.LogToChatAll(tempAutoBalanceMessage.ReplaceTags());
        }
    }
}
