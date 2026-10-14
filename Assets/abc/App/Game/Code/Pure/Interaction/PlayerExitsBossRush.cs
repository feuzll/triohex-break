using abc.App.Game.Code.Pure.Boss;
using R3;

namespace abc.App.Game.Code.Pure.Interaction
{
    public static class PlayerExitsBossRush
    {
        public static Observable<Unit> OnNew => NewEvent;

        private static readonly Subject<Unit> NewEvent = new();

        internal static void Raise(Gameplay.Runtime gameplayRuntime, Rush rush)
        {
            rush.IsFightingProperty.Value = false;
            rush.PendingFights.Clear();
            rush.DefeatSubscriptions.Clear();
        }
    }
}