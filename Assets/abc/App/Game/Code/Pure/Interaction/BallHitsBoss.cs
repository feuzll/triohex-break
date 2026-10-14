using R3;

namespace abc.App.Game.Code.Pure.Interaction
{
    public static class BallHitsBoss
    {
        public static Observable<(Gameplay, Bosses.Boss, Ball)> New => newEvent;

        private static Subject<(Gameplay, Bosses.Boss, Ball)> newEvent = new();
        
        internal static void Raise(Bosses.Boss.Runtime boss, Ball ball)
        {
            if (ball == null) return;
            boss.ApplyDamage(ball.Strength);
            newEvent.OnNext((boss.GameplayRuntime.Context, boss.Instance, ball));
        }
    }
}