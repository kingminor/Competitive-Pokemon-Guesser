using Microsoft.AspNetCore.SignalR;
using ShadowDex.Managers;
using ShadowDex.Models;

namespace ShadowDex.Hubs
{
    public class GameHub : Hub
    {
        private readonly IHubContext<GameHub> _hubContext;

        public GameHub(IHubContext<GameHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task CreateGame(int maxPlayers, int timeBeforeReveal, string nickname){
            string gameID = SessionManager.GenerateGameId();
            GameSession generatedSession = new GameSession(gameID, maxPlayers, timeBeforeReveal, _hubContext);
            SessionManager.Sessions.TryAdd(gameID, generatedSession);
        }

        // public async Task JoinGame(string nickname, string gameID)
        // {
            
        // }
    }
}