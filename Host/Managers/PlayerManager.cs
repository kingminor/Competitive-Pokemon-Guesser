using ShadowDex.Models;
using System.Collections.Concurrent;

namespace ShadowDex.Managers{
    public static class PlayerManager {
        public static ConcurrentDictionary<string, Player> ConnectedPlayers = new ConcurrentDictionary<string, Player>();
    }
}