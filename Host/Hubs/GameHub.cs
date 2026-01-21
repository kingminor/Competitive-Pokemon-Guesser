using Microsoft.AspNetCore.SignalR;
using ShadowDex.Managers;
using ShadowDex.Models;
using System.Collections.Concurrent;

namespace ShadowDex.Hubs
{
    public class GameHub : Hub
    {
        private readonly IHubContext<GameHub> _hubContext;
        private ConcurrentDictionary<string, Player> ConnectedPlayers = new ConcurrentDictionary<string, Player>();

        public GameHub(IHubContext<GameHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public override async Task OnConnectedAsync() {
            string connectionId = Context.ConnectionId;
            var player = new Player(connectionId);
            ConnectedPlayers.TryAdd(connectionId, player);

            await Clients.Caller.SendAsync("Connected", player.PlayerID);

            await base.OnConnectedAsync();
        }

        public async Task EditNickName(string playerID, string nickname) {
            ConnectedPlayers.TryGetValue(playerID, out var playerData);
            if(playerData != null) {
                playerData.NickName = nickname;
                await Clients.Caller.SendAsync("Nickname Changed");
            }
            else {
                await Clients.Caller.SendAsync("Error", $"Player Not Found With ID: {playerID}");
            }
        }

        public async Task CreateGame(int maxPlayers, int timeBeforeReveal, string playerID){

            var gameID = SessionManager.CreateGame(maxPlayers, timeBeforeReveal, _hubContext);
            
            ConnectedPlayers.TryGetValue(playerID, out var player);
            SessionManager.AddPlayerToGame(gameID, player);

            await Clients.Caller.SendAsync("Game Created", gameID);
            await Groups.AddToGroupAsync(Context.ConnectionId, gameID);
        }

        public async Task JoinGame(string playerID, string gameID)
        {
            ConnectedPlayers.TryGetValue(playerID, out var player);
            if(SessionManager.AddPlayerToGame(gameID, player)){
                await Clients.Caller.SendAsync("Joined Game");
                await Groups.AddToGroupAsync(Context.ConnectionId, gameID);
            }
            else {
                await Clients.Caller.SendAsync("Error");
            }
        }

        public async Task StartGame(string playerID) {
            SessionManager.StartGame(playerID);
        }

        public async Task GuessPokemon(string playerID, string guess) {
            var gameSession = SessionManager.GetGameFromPlayerID(playerID);
            if (gameSession == null)
            {
                await Clients.Caller.SendAsync("Error");
                return;
            }
            
            
            var result = gameSession.CheckGuess(guess);
            if(result != null) {
                await Clients.Group(gameSession.GameID).SendAsync("End Game", "Correct Guess", playerID, result);
            }
            else {
                await Clients.Caller.SendAsync("Incorrect Guess");
            }
        }
    }
}