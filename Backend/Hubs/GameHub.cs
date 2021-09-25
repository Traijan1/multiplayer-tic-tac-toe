using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace Backend.Hubs;

public class GameHub : Hub {
    GameManager Manager { get; set; }

    public GameHub(GameManager manager) {
        Manager = manager;
    }

    public async Task JoinGame(string gameId) {
        Game game = Manager.GetGame(gameId);

        if(game is null) {
            await Clients.Caller.SendAsync("Error", "The requested game don't exists!");
            return;
        }

        game.AddUser(Context.ConnectionId);        
        await Groups.AddToGroupAsync(Context.ConnectionId, game.Id);

        if (game.Users.Count == 2) {
            bool isPlayerOneBeginner = new Random().Next(2) == 0;

            await Clients.Client(game.Users[0]).SendAsync("SetMover", isPlayerOneBeginner);
            game.SetPlayer(game.Users[0], isPlayerOneBeginner ? 'X' : 'O');

            await Clients.Client(game.Users[1]).SendAsync("SetMover", !isPlayerOneBeginner);
            game.SetPlayer(game.Users[1], !isPlayerOneBeginner ? 'X' : 'O');

            await Clients.Group(game.Id).SendAsync("GetGame", game.MatchAsJson);
        }
    }

    public async Task CheckMove(string gameId, int x, int y) {
        Game game = Manager.GetGame(gameId);

        if(game is null) {
            await Clients.Caller.SendAsync("Error", "Game couldn't be found");
            return;
        }

        bool isValidMove = game.Match.SetChar(x, y);

        if (isValidMove) {
            await Clients.Group(game.Id).SendAsync("GetGame", game.MatchAsJson);
            await Clients.Client(game.Users.FindAll(connId => connId != Context.ConnectionId)[0]).SendAsync("SetMover", true);
        }
        else
            await Clients.Caller.SendAsync("SetMover", true);
    }
}
