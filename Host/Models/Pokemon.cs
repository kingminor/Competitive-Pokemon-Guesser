namespace ShadowDex.Models{
    public class Pokemon {
        public int PokedexNumber { get; set; }
        public string Name { get; set; }

        public override string ToString(){
            return $"{PokedexNumber}: {Name}";
        }
    }
}