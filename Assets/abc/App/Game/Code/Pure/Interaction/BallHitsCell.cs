using System;
using R3;

namespace abc.App.Game.Code.Pure.Interaction
{
    public static class BallHitsCell
    {
        public static Observable<(Gameplay, Cell, Ball)> New => newEvent;

        private static Subject<(Gameplay, Cell, Ball)> newEvent = new();
        
        internal static void Raise(Gameplay context, Cell cell, Ball ball)
        {
            if (ball != null) //TODO: sometimes this somehow happen?  
                newEvent.OnNext((context, cell, ball));
        }
    }
}