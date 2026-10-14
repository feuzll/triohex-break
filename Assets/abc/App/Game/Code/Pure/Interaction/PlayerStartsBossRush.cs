using abc.App.Game.Code.Pure.Boss;
using ObservableCollections;
using R3;

namespace abc.App.Game.Code.Pure.Interaction
{
    public static class PlayerStartsBossRush
    {
        public static Observable<Unit> OnNew => NewEvent;

        private static readonly Subject<Unit> NewEvent = new();

        internal static bool Try(Gameplay.Runtime gameplayRuntime, Rush rush)
        {
            if (rush.IsFighting.CurrentValue) return false;
            if (rush.Level.CurrentValue <= 0) rush.LevelProperty.Value = 1;
            
            //guard resource deficit
            var requiredSuperShards = rush.Level.CurrentValue * 1000;
            var spendResult =
                gameplayRuntime.Context.Resources[Resource.Type.SS].TrySpend(requiredSuperShards);
            if (spendResult == false) return false;
            
            //plan boss fights
            rush.PendingFights.Clear();
            for (var i = 0; i < rush.Level.CurrentValue; i++)
                rush.PendingFights.Push(
                    (rush.BossesCycle[i%rush.BossesCycle.Count], 1000000 * i));
            rush.IsFightingProperty.Value = true;
            rush.PendingFights.ObserveCountChanged(true)
                .Where(c => c == 0)
                .Take(1)
                .Subscribe(_ =>
                {
                    if (!rush.IsFighting.CurrentValue) return;
                    rush.LevelProperty.Value++; //reward
                    rush.IsFightingProperty.Value = false;
                });
            foreach (var f in rush.PendingFights)
                f.Item1.Defeated.Subscribe(_ => 
                        HandleDefeatOf(rush, f.Item1))
                    .AddTo(rush.DefeatSubscriptions);
            return true;
        }

        private static void HandleDefeatOf(Rush rush, Base boss)
        {
            if (!rush.PendingFights.TryPeek(out var fight) 
                || boss != fight.Item1) return;
            
            rush.PendingFights.Pop();
            if (rush.PendingFights.Count == 0) 
                rush.IsFightingProperty.Value = false;
            else
            {
                var next = rush.PendingFights.Peek(); 
                next.Item1.HealthProperty.Value = next.Item2;
            }
        }
    }
}