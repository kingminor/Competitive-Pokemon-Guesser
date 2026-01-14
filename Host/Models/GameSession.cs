using System.Collections.Generic;
using System.Timers;
using Microsoft.AspNetCore.SignalR;
using ShadowDex.Hubs;

namespace ShadowDex.Models {
    public class GameSession {
        public string GameID {get;set;}
        public bool GameStarted {get; private set;}

        private readonly IHubContext<GameHub> _hubContext;
        private List<Player> _players;

        //Session Settings
        private int _maxPlayers;
        private int _timeBeforeReveal;
        //private int _pokemonPerRound; MIGHT ADD BACK, CURRENTLY REMOVED

        //Private Into
        //private Pokemon currentPokemon;
        private System.Timers.Timer timer;

        public GameSession (string gameID, int maxPlayers, int timeBeforeReveal, IHubContext<GameHub> hubContext){
            GameID = gameID;
            _maxPlayers = maxPlayers;
            _timeBeforeReveal = timeBeforeReveal;
            _hubContext = hubContext;
            GameStarted = false;
            timer = new System.Timers.Timer(timeBeforeReveal * 1000);
            timer.AutoReset = false;
            timer.Elapsed += OnTimeElapsed;
        }

        public bool AddPlayer(Player player){
            if(_players.Count < _maxPlayers){
                _players.Add(player);
                return true;
            }
            else return false;
            
        }

        public async Task StartRound(){
            GameStarted = true;
            await _hubContext.Clients.Group(GameID).SendAsync("Game Started", "ImageURL");
        }

        private void OnTimeElapsed(object sender, ElapsedEventArgs e)
        {
            GameStarted = false;
        }

    }
}