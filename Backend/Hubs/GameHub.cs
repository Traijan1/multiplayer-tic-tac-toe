using Microsoft.AspNetCore.SignalR;
using Model;
using System;
using System.Diagnostics;
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

    /// <summary>
    /// Checks if the move is valid, then check if a winner can be decided, if not, then send the next mover
    /// </summary>
    /// <param name="gameId"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public async Task CheckMove(string gameId, int x, int y) {
        Game game = Manager.GetGame(gameId);

        if(game is null) {
            await Clients.Caller.SendAsync("Error", "Game couldn't be found");
            return;
        }

        bool isValidMove = game.Match.SetChar(x, y);

        if (isValidMove) {
            var state = game.Match.DidSomeoneWon();
            bool isWinner = state != TicTacToeState.OnGoing;

            await Clients.Group(game.Id).SendAsync("GetGame", game.MatchAsJson);

            if (isWinner) {
                string message = state == TicTacToeState.Win ? $"Player {game.Match.CurrentChar} has won" : "It's a draw";
                await Clients.Group(game.Id).SendAsync("Winner", message);

                return; 
            }

            game.Match.NextRound();
            await Clients.Client(game.Users.FindAll(connId => connId != Context.ConnectionId)[0]).SendAsync("SetMover", true);
        }
        else
            await Clients.Caller.SendAsync("SetMover", true);
    }

    /// <summary>
    /// Removes the player from the Game and if it was the last one, remove the game
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public override async Task OnDisconnectedAsync(Exception exception) {
        await base.OnDisconnectedAsync(exception);

        Game game = Manager.GetGameByConnectionId(Context.ConnectionId);
        bool didWork = game.Users.Remove(Context.ConnectionId);

        Debug.Assert(didWork);

        if (game.Users.Count == 0) 
            Manager.Games.Remove(game);
    }
}
