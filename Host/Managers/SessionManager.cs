using System.Collections.Concurrent;
using System.Linq;
using ShadowDex.Models;
using ShadowDex.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ShadowDex.Managers {
    public static class SessionManager {
        private static ConcurrentDictionary<string, GameSession> Sessions = new ConcurrentDictionary<string, GameSession>();
        private static ConcurrentDictionary<string, GameSession> playerIDToGameSession = new ConcurrentDictionary<string, GameSession>();

        private static Random random = new Random();

        public static string GenerateGameId() {
            // Generates a random Alphanumeric String
            // Credit: Stack Overflow users dtb and Wai Ha Lee https://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            const int length = 6;
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string CreateGame(int maxPlayers, int timeBeforeReveal, IHubContext<GameHub> _hubContext) {
            string gameID = GenerateGameId();
            GameSession generatedSession = new GameSession(gameID, maxPlayers, timeBeforeReveal, _hubContext);
            Sessions.TryAdd(gameID, generatedSession);
            return(gameID);
        }

        public static bool AddPlayerToGame(string gameID, Player player){
            if(Sessions.TryGetValue(gameID, out var gameSession)){
                if(gameSession.AddPlayer(player)) {
                    playerIDToGameSession.TryAdd(player.PlayerID, gameSession);
                    return true;
                }
                else return false;
            }
            else return false;
        }

        public static GameSession? GetGameFromGameID(string gameID) {
            Sessions.TryGetValue(gameID, out var gameSession);
            return gameSession;
        }

        public static GameSession? GetGameFromPlayerID(string playerID){
            playerIDToGameSession.TryGetValue(playerID, out var gameSession);
            return gameSession;
        }

        public static bool StartGame(string playerID) {
            if(playerIDToGameSession.TryGetValue(playerID, out var gameSession)){
                gameSession.StartRound();
                return true;
            }
            else return false;
        }
    }
}