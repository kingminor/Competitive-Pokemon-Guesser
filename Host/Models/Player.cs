using Microsoft.AspNetCore.SignalR;

namespace ShadowDex.Models{
    public class Player {
        public string PlayerID {get; set;}
        public string? NickName {get;set;}
        public string ConnectionID {get;set;}

        public Player(string connectionID) {
            PlayerID = Guid.NewGuid().ToString();
            ConnectionID = connectionID;
        }
    }
}