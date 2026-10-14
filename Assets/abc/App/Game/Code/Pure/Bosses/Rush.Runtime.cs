namespace abc.App.Game.Code.Pure.Bosses
{
    public partial class Rush
    {
        public class Runtime
        {
            internal Rush State { get; private set; }
            internal Runtime(Gameplay.Runtime gameplayRuntime, Rush rush)
            {
                State = rush;
                State.BossesCycle = new();
                State.IsFightingProperty = new(false);
                State.PendingFights = new();
                State.DefeatSubscriptions = new();
                foreach (var boss in rush.PremadeBossesCycle)
                {
                    State.BossesCycle.Add(new Boss.Runtime(gameplayRuntime, boss.Pure));
                }
            }
        }
    }
}