using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Timers;
using Microsoft.AspNetCore.SignalR;
using ShadowDex.Hubs;
using ShadowDex.Managers;

namespace ShadowDex.Models {
    public class GameSession {
        public string GameID {get;set;}
        public bool GameStarted {get; private set;}

        // Useful Things
        private readonly IHubContext<GameHub> _hubContext;
        private List<Player> _players = new List<Player>();

        //Session Settings
        private int _maxPlayers;
        private int _timeBeforeReveal;

        //Private Into
        private Pokemon currentPokemon;
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
            if(GameStarted) return;
            currentPokemon = PokemonManager.GetRandomPokemon();
            string ImageURL = $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/{currentPokemon.PokedexNumber}.png";
            GameStarted = true;
            await _hubContext.Clients.Group(GameID).SendAsync("Game Started", ImageURL);

            timer.Interval = _timeBeforeReveal * 1000; // milliseconds
            timer.Start();
        }

        // Returns null if wrong, returns name if right
        public string CheckGuess(string guess) {
            if(string.IsNullOrWhiteSpace(guess)) return null;
            if(currentPokemon == null) return null;

            if(guess.ToLower() == currentPokemon.Name.ToLower()){
                EndGame();
                return currentPokemon.Name;
            }
            else return null;
        }

        private async void OnTimeElapsed(object sender, ElapsedEventArgs e)
        {
            if (currentPokemon == null) return;

            var pokemonName = currentPokemon.Name;
            EndGame();
            currentPokemon = null;

            await _hubContext.Clients.Group(GameID).SendAsync("End Game", "Time Up", null, pokemonName);
        }


        public bool IsPlayerInGame(string playerID) {
            bool flag = false;
            foreach (Player player in _players) {
                if (player.PlayerID == playerID) flag = true;
            }
            return flag;
        }

        private void EndGame() {
            timer.Stop();
            GameStarted = false;
        }

    }
}