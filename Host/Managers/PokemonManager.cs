using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using ShadowDex.Models;

namespace ShadowDex.Managers
{
    public static class PokemonManager
    {
        private static readonly List<Pokemon> _pokemonList;
        private static readonly Random _random = new Random();

        static PokemonManager()
        {
            try
            {
                var jsonText = File.ReadAllText("./data/allpokemon.json");

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                // Deserialize into JsonElement array
                var rawList = JsonSerializer.Deserialize<List<JsonElement>>(jsonText, options);

                // Select only the properties defined in Pokemon
                _pokemonList = rawList?
                    .Select(p =>
                    {
                        int pokedexNumber = 0;
                        string name = "Unknown";

                        if (p.TryGetProperty("pokedexNumber", out var numProp))
                            pokedexNumber = numProp.GetInt32();

                        if (p.TryGetProperty("name", out var nameProp))
                            name = nameProp.GetString() ?? "Unknown";

                        return new Pokemon
                        {
                            PokedexNumber = pokedexNumber,
                            Name = name
                        };
                    })
                    .Where(p => p.PokedexNumber > 0 && !string.IsNullOrWhiteSpace(p.Name))
                    .ToList() ?? new List<Pokemon>();

                if (_pokemonList.Count == 0)
                    Console.WriteLine("Warning: No Pokémon loaded from JSON!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading Pokémon JSON: " + ex.Message);
                _pokemonList = new List<Pokemon>();
            }
        }

        public static Pokemon GetRandomPokemon()
        {
            if (_pokemonList.Count == 0)
                throw new InvalidOperationException("No Pokémon loaded.");

            return _pokemonList[_random.Next(_pokemonList.Count)];
        }
    }
}
