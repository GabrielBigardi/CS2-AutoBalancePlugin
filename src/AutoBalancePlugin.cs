using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace AutoBalancePlugin;

public class AutoBalancePlugin : BasePlugin
{
    public override string ModuleName => "Auto Balance Plugin";
    public override string ModuleVersion => "0.1.0";
    public override string ModuleAuthor => "hTx";

    public override void Load(bool hotReload)
    {
        RegisterEventHandler<EventRoundEnd>(OnRoundEnd);
        LogToConsole(ConsoleColor.Green, $"{ModuleName} version {ModuleVersion} loaded");
    }

    public override void Unload(bool hotReload)
    {
        LogToConsole(ConsoleColor.Green, $"{ModuleName} version {ModuleVersion} unloaded");
    }
    
    private HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
    {
        LogToConsole(ConsoleColor.Green, $"New round started, fetching players");
        var players = Utilities.GetPlayers();
        
        if (players.Count <= 0)
            return HookResult.Continue;

        var ctPlayers = players.FindAll(x => x.TeamNum == (int)CsTeam.CounterTerrorist);
        var trPlayers = players.FindAll(x => x.TeamNum == (int)CsTeam.Terrorist);

        var difference = Math.Abs(ctPlayers.Count - trPlayers.Count);
        var maximumAllowedDifference = 1;

        if (difference > maximumAllowedDifference)
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
                
                playerToSend.ChangeTeam(teamToSend);
            }
        }

        return HookResult.Continue;
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
}
