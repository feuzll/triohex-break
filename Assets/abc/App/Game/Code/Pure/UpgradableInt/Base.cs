using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using R3;

namespace abc.App.Game.Code.Pure.UpgradableInt
{
    [Serializable]
    public abstract class Base : IUpgradableInt
    {
        protected Runtime _runtime;
        protected Gameplay.Runtime _gameplayRuntime;
        
        
#if ODIN_INSPECTOR
        [ShowInInspector]
        [ReadOnly]
#endif
        public virtual int Value => 0;

#if ODIN_INSPECTOR
        [ShowInInspector] [ReadOnly]
#endif
        public int NextStepCost =>
            (int)(BaseCostPerStep *
                  Math.Pow(CostPreStepMultiplier, Step));
        

        protected int _step; // save target
        [OdinSerialize]
        public int Step
        {
            get => _step;
            protected set
            {
                if (MaxStep > 0 && value > MaxStep)
                    return;
                _step = value;
                NewStep?.Invoke(value);
            }
        }

        public bool IsMaxed => (MaxStep > 0 && Step >= MaxStep);

        public event Action<int> NewStep;
        void IUpgradableInt.AdvanceStep()
        {
            Step++;
        }
        
        [OdinSerialize] public int StartingValue { get; protected set; }
        [OdinSerialize] public int MaxStep { get; protected set; }
        [OdinSerialize] public int BaseCostPerStep { get; protected set; }
        [OdinSerialize] public float CostPreStepMultiplier { get; protected set; }
        [OdinSerialize] public Resource.Type CostResourceType { get; protected set; }
        
        
        [Serializable]
        public class Runtime : IIntProperty.Runtime
        {

            public Runtime(Gameplay.Runtime gameplayRuntime, Base instance, IPropertyType type) 
                : base(gameplayRuntime, instance, type)
            {
                if (instance._runtime != null || instance._gameplayRuntime != null) 
                    throw new NotSupportedException();
                
                instance._runtime = this;
                instance._gameplayRuntime = gameplayRuntime;
                
                if (instance.StartingValue <= 0)
                    instance.StartingValue = 1;
                
                if (instance.MaxStep > 0 && instance.Step > instance.MaxStep)
                    instance.Step = instance.MaxStep;
                
                instance.Step = 0;
                
                gameplayRuntime.Context.Level.Subscribe(level =>
                {
                    if (level != 1) return;
                    if (type == gameplayRuntime.Context.AnyBallPowerMultiplicatorType 
                        || type == gameplayRuntime.Context.AnyBallAcceleratorType)
                        return;
                    instance.Step = 0;
                });
            }
        }
    }
}