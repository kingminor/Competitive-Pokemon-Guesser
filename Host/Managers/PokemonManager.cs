using System;
using System.IO;
using System.Text.Json;
using ShadowDex.Models;

namespace ShadowDex.Managers{
    public static class PokemonManager {
        private static readonly List<Pokemon> _pokemonList;
        private static readonly Random _random = new Random();

        static PokemonManager () {
            try {
                var jsonText = File.ReadAllText("./data/allpokemon.json");

                var options = new JsonSerializerOptions {
                    PropertyNameCaseInsensitive = true
                };

                var rawList = JsonSerializer.Deserialize<List<JsonElement>>(jsonText, options);

                _pokemonList = rawList.Select(p => new Pokemon {
                    PokedexNumber = p.GetProperty("pokedexNumber").GetInt32(),
                    Name = p.GetProperty("name").GetString()

                }).ToList();
            }
            catch (Exception ex) {
                Console.WriteLine("Error loading Pokémon JSON: " + ex.Message);
                _pokemonList = new List<Pokemon>();
            }
        }

        public static Pokemon GetRandomPokemon() {
            return _pokemonList[_random.Next(_pokemonList.Count)];
        }
    }
}