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
            Player newPlayer = new Player(nickname);
            generatedSession.AddPlayer(newPlayer);
            await Clients.Caller.SendAsync("Game Created", gameID, newPlayer.PlayerID);
            await Groups.AddToGroupAsync(Context.ConnectionId, gameID);
        }

        public async Task JoinGame(string nickname, string gameID)
        {
            Player newPlayer = new Player(nickname);
            if(SessionManager.Sessions.TryGetValue(gameID, out GameSession session)){
                if(session.AddPlayer(newPlayer)) {
                    await Clients.Caller.SendAsync("Joined Game", newPlayer.PlayerID);
                    await Groups.AddToGroupAsync(Context.ConnectionId, gameID);
                }
                else {
                    await Clients.Caller.SendAsync("Error", "Session Full");
                }
            }
            else {
                await Clients.Caller.SendAsync("Error", "Session Not Found");
            }
        }

        public async Task StartGame(string playerID, string gameID) {
            try {
                SessionManager.Sessions.TryGetValue(gameID, out var game);
                if(game.IsPlayerInGame(playerID)) {
                    game.StartRound();
                }
                else {
                    await Clients.Caller.SendAsync("Error", $"You are not in the group: {gameID}");
                }
            }
            catch (Exception ex){
                Console.WriteLine("Error Starting Game: " + ex);
                await Clients.Caller.SendAsync("Error", ex);
            }
        }

        public async Task GuessPokemon(string playerID, string gameID, string guess) {
            if (!SessionManager.Sessions.TryGetValue(gameID, out var game))
            {
                await Clients.Caller.SendAsync("Error", $"Game {gameID} not found.");
                return;
            }
            
            if(game.IsPlayerInGame(playerID)) {
                var result = game.CheckGuess(guess);
                if(result != null) {
                    await Clients.Group(gameID).SendAsync("End Game", "Correct Guess", playerID, result);
                }
                else {
                    await Clients.Caller.SendAsync("Incorrect Guess");
                }

            }
            else {
                await Clients.Caller.SendAsync("Error", $"You are not in the game: {gameID}");
            }
        }
    }
}