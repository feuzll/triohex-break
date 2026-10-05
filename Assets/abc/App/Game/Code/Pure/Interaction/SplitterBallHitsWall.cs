using System;
using R3;

namespace abc.App.Game.Code.Pure.Interaction
{
    public static class SplitterBallHitsWall
    {
        public static Observable<(Gameplay, Ball)> New => newEvent;

        private static Subject<(Gameplay, Ball)> newEvent = new();
        
        internal static void Raise(Gameplay context, Ball ball)
        {
            if (ball.Variation != Ball.Type.Splitter)
                throw new ArgumentException();
            newEvent.OnNext((context, ball));
        }
    }
}