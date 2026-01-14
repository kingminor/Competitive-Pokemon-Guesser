using System.Collections.Concurrent;
using System.Linq;
using ShadowDex.Models;

namespace ShadowDex.Managers {
    public static class SessionManager {
        public static ConcurrentDictionary<string, GameSession> Sessions = new ConcurrentDictionary<string, GameSession>();

        private static Random random = new Random();

        public static string GenerateGameId() {
            // Generates a random Alphanumeric String
            // Credit: Stack Overflow users dtb and Wai Ha Lee https://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            const int length = 6;
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}