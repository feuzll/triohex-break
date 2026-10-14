using R3;

namespace abc.App.Game.Code.Pure.Boss
{
    public partial class Base
    {
        public class Runtime
        {
            private readonly Base _instance;

            public Runtime(Gameplay.Runtime context, Base instance)
            {
                _instance = instance;
                if (_instance.HealthProperty.CurrentValue < 1000000) 
                    _instance.HealthProperty.Value = 1000000;
                _instance.Defeated = _instance.HealthProperty.Select(h => h <= 0)
                    .DistinctUntilChanged()
                    .Where(b => b)
                    .AsUnitObservable();
            }

            
        }

    }
}