using abc.App.Game.Code.Pure.Bosses;
using ObservableCollections;
using R3;

namespace abc.App.Game.Code.Pure.Interaction
{
    public static class PlayerStartsBossRush
    {
        public static Observable<Unit> OnNew => NewEvent;

        private static readonly Subject<Unit> NewEvent = new();

        internal static bool Try(Gameplay.Runtime gameplayRuntime, Rush.Runtime rushRuntime)
        {
            //TODO: introduce InRush along with InFight - we should quit manually
            //TODO: what about endless splitter balls after fight though...
            
            //TODO: do we respecting possible prestige mid fight?
            
            //TODO: sniper balls retarget to bosses
            
            var rush = rushRuntime.State;
            if (rush.IsFighting.CurrentValue) return false;
            if (rush.Level.CurrentValue <= 0) rush.LevelProperty.Value = 1;
            
            //guard resource deficit
            var requiredSuperShards =
                rush.Level.CurrentValue * gameplayRuntime.Context.SSCostPerBossRushLevel;
            var spendResult =
                gameplayRuntime.Context.Resources[Resource.Type.SS].TrySpend(requiredSuperShards);
            if (spendResult == false) return false;
            
            //plan boss fights
            rush.PendingFights.Clear();
            for (var i = 0; i < rush.Level.CurrentValue; i++)
                rush.PendingFights.Enqueue(
                    (rush.BossesCycle[i%rush.BossesCycle.Count], 1000000 * i));
            var entryBossFight = rush.PendingFights.Peek();
            entryBossFight.Item1.Revive(entryBossFight.Item2);
            rush.IsFightingProperty.Value = true;
            rush.RewardSubscription = rush.PendingFights.ObserveCountChanged(true)
                .Where(c => c == 0)
                .Take(1)
                .Subscribe(_ =>
                {
                    rush.LevelProperty.Value++; //reward
                    gameplayRuntime.Context.Resources[Resource.Type.Skill].AmountProperty.Value++;
                    rush.IsFightingProperty.Value = false;
                });
            foreach (var f in rush.PendingFights)
                f.Item1.Instance.Defeated.Subscribe(_ => 
                        HandleDefeatOf(rush, f.Item1))
                    .AddTo(rush.DefeatSubscriptions);
            return true;
        }

        private static void HandleDefeatOf(Rush rush, Boss.Runtime boss)
        {
            if (!rush.PendingFights.TryPeek(out var fight) 
                || boss != fight.Item1) return;
            
            
            rush.PendingFights.Dequeue();
            if (rush.PendingFights.Count == 0) 
                rush.IsFightingProperty.Value = false;
            else
            {
                var next = rush.PendingFights.Peek(); 
                next.Item1.Revive(next.Item2);
            }
        }
    }
}