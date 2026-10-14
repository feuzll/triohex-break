using abc.App.Game.Code.Pure.Bosses;
using R3;

namespace abc.App.Game.Code.Pure.Interaction
{
    public static class PlayerExitsBossRush
    {
        public static Observable<Unit> OnNew => NewEvent;

        private static readonly Subject<Unit> NewEvent = new();

        internal static void Raise(Gameplay.Runtime gameplayRuntime, Rush.Runtime rushRuntime)
        {
            var rush = rushRuntime.State;
            rush.IsFightingProperty.Value = false;
            rush.PendingFights.Clear();
            rush.DefeatSubscriptions.Clear();
        }
    }
}