using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Backend.Hubs;

public class GameHub : Hub {
    GameManager Manager { get; set; }

    public GameHub(GameManager manager) {
        Manager = manager;
    }

    public async Task JoinGame(string gameId) {
        Game game = Manager.Games.Find(game => game.Id == gameId);

        if(game is null) {
            await Clients.Caller.SendAsync("Error", "The requested game don't exists!");
            return;
        }

        game.AddUser(Context.ConnectionId);
        await Clients.Caller.SendAsync("InvitationLink", $"Invite your friends: {Context.GetHttpContext().Request.PathBase}/invite/{game.Id}");
    }
}
