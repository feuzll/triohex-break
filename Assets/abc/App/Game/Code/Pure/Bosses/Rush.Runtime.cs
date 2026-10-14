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
                foreach (var boss in rush.PremadeBossesCycle)
                {
                    State.BossesCycle.Add(new Boss.Runtime(gameplayRuntime, boss));
                }
            }
        }
    }
}