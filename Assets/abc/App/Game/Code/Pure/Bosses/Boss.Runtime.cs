using R3;

namespace abc.App.Game.Code.Pure.Bosses
{
    public partial class Boss
    {
        public class Runtime
        {
            internal readonly Gameplay.Runtime GameplayRuntime;
            internal readonly Boss Instance;

            public Runtime(Gameplay.Runtime gameplayRuntime, Boss instance)
            {
                GameplayRuntime = gameplayRuntime;
                Instance = instance;
                Instance.Driver = this;
                Instance.HealthProperty = new(0);
                Instance.Defeated = Instance.HealthProperty.Select(h => h <= 0)
                    .DistinctUntilChanged()
                    .Where(b => b)
                    .AsUnitObservable();
            }

            internal void ApplyDamage(int amount)
            {
                if (amount > Instance.Health.CurrentValue) 
                    amount = Instance.Health.CurrentValue;
                Instance.HealthProperty.Value -= amount;
            }

            internal void Revive(int newHealth)
            {
                if (newHealth <= 0) newHealth = 999999;
                Instance.HealthProperty.Value =  newHealth;
            }
        }

    }
}