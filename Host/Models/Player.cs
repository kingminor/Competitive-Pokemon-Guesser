using Microsoft.AspNetCore.SignalR;

namespace ShadowDex.Models{
    public class Player {
        public string PlayerID {get; set;}
        public string NickName {get;set;}

        public Player(string nickname) {
            NickName = nickname;
            PlayerID = Guid.NewGuid().ToString();
        }
    }
}