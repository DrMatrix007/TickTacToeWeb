using Microsoft.AspNetCore.SignalR;
using TickTacToeWeb.Models;
using System.Timers;

namespace TickTacToeWeb.Hubs;

public class GamesHub : Hub
{

    private static readonly Dictionary<Game, (string? Player1, string? Player2)> games = new Dictionary<Game, (string? Player1, string? Player2)>();

    private readonly Random rand = new Random();

    public async Task Create()
    {
        var game = new Game(async (a) => { Console.WriteLine("deleted a game!"); await StopGame(a); games.Remove(a); });

        games.Add(game, (Context.ConnectionId, null));
        await Clients.Caller.SendAsync("GetGuidGame", game.Id);
    }

    private async Task StopGame(Game game)
    {
        var clients = games[game];
        if(clients.Player2 != null)
        {
            await (Clients.Client(clients.Player2).SendAsync("GameStop", "game outdated"));
        }
        if(clients.Player1 != null)
        {
            await (Clients.Client(clients.Player1).SendAsync("GameStop", "game outdated"));
        }
    }

    public async Task Join(string guidString)
    {
        var isGood = Guid.TryParse(guidString, out Guid guid);
        if(!isGood)
        {
            await Clients.Caller.SendAsync("GetErrorJoin", "does not exsist");
            return;
        }


        //var game = (await Task.Run(()=>games.ToList())).FirstOrDefault(game => game.Key.Id == Guid.Parse(guid));
        var have = games.Any(game => game.Key.Id == guid);
        if(have)
        {
            var game = games.First(game => game.Key.Id == guid);
            if(game.Value.Player2 == null)
            {
                games[game.Key] = (game.Value.Player1, Context.ConnectionId);

                var whoX = rand.Next(0, 100) > 50;
                var whoFirst = rand.Next(0, 100) > 50;



                await Clients.Caller.SendAsync("Ready", guid, whoX ? "O" : "X", game.Key.current == Game.CellType.O ? "O" : "X");
                await (Clients.Client(game.Value.Player1!))!.SendAsync("Ready", guid, !whoX ? "O" : "X", game.Key.current == Game.CellType.O ? "O" : "X");
            }
            else
            {
                await Clients.Caller.SendAsync("GetErrorJoin", "already taken");
            }

        }
        else
        {
            await Clients.Caller.SendAsync("GetErrorJoin", "game does not exist");

        }
    }
    public async Task PlayMove(string guidString, string Player, int x, int y)
    {
        var guid = Guid.Parse(guidString);
        if(games.Any(a => a.Key.Id == guid))
        {
            var game = games.First(a => a.Key.Id == (guid));
            if(game.Value.Player1 == Context.ConnectionId || game.Value.Player2 == Context.ConnectionId)
            {
                var currentPlayer = (game.Key.current == Game.CellType.O ? "O" : "X");
                if(Player.ToUpper() == currentPlayer)
                {
                    var winner = game.Key.PlayNext(x, y);

                    await (Clients.Client(game.Value.Player2!).SendAsync("GetBoard", game.Key.board, currentPlayer));
                    await (Clients.Client(game.Value.Player1!).SendAsync("GetBoard", game.Key.board, currentPlayer));
                    if(winner != Game.CellType.None)
                    {
                        await Task.Delay(200);
                        await (Clients.Client(game.Value.Player2!).SendAsync("Winner", winner));
                        await (Clients.Client(game.Value.Player1!).SendAsync("Winner", winner));
                        games.Remove(game.Key);
                    }

                }
            }
            else
            {
                await Clients.Caller.SendAsync("GetGameError", "not your game!");
            }
        }
        else
        {

        }
    }
}
