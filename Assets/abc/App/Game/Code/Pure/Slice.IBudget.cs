namespace abc.App.Game.Code.Pure
{
    public partial class Slice
    {
        public interface IBudget
        {
            bool CanStart { get; }                 // e.g. mana >= minimum to begin
            float Request(float worldLength);      // returns granted length in [0, worldLength], and spends the mana for it
        }
    }
    
}