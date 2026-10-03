using R3;

namespace abc.App.Game.Code.Pure.Interaction
{
    public static class SliceHitsCell
    {
        public static Observable<(Slice, Cell)> New => newEvent;

        private static Subject<(Slice, Cell)> newEvent = new();
        
        internal static void Raise(Slice context, Cell cell)
        {
            newEvent.OnNext((context, cell));
        }
    }
}