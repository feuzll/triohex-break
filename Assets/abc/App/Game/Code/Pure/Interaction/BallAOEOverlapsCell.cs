using System;
using R3;

namespace abc.App.Game.Code.Pure.Interaction
{
    public static class BallAOEOverlapsCell
    {
        public static Observable<(Gameplay, Cell, int)> New => newEvent;

        private static Subject<(Gameplay, Cell, int)> newEvent = new();
        
        internal static void Raise(Gameplay context, Cell cell, int damage)
        {
            if (cell == null || context == null)
                throw new ArgumentException();
            
            newEvent.OnNext((context, cell, damage));
        }
    }
}