using Microsoft.AspNetCore.SignalR;

namespace BITool.Models.SignalR
{
    public interface IGameClient
    {
        Task GameStarted(string gameCode);

        Task<Player> PlayerJoined(string playerName);

        Task ToSendAsync();
    }

    public class GameClient : Hub, IGameClient
    {
        public GameClient(
            )
        {
        }

        public async Task GameStarted(string gameCode)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameCode);
        }

        public async Task<Player> PlayerJoined(string playerName)
        {
            //var game = await GetGameByCode(gameCode);
            Console.WriteLine($"trigger {nameof(PlayerJoined)} {playerName}");
            var random = new Random();
            var player = new Player
            {
                GameId = random.Next(1, 6),
                Name = playerName,
                JoinedOn = DateTime.UtcNow,
                IsHost = false
            };
            return player;
        }

        public async Task ToSendAsync()
        {
            Console.WriteLine($"trigger {nameof(ToSendAsync)}");
            var random = new Random();
            var player = new Player
            {
                GameId = random.Next(1, 6),
                Name = nameof(ToSendAsync),
                JoinedOn = DateTime.UtcNow,
                IsHost = false
            };
            await Clients.All.SendAsync("getPlayer", player);
        }
    }
}