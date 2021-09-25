using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Backend.Hubs;

public class EchoHub : Hub {
    public async Task Echo(string message) {
        await Clients.Caller.SendAsync("SendEcho", message);
    }
}
