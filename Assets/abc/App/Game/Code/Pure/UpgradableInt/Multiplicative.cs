using System;
using R3;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

using Math =
#if UNITY_5_3_OR_NEWER
    UnityEngine.Mathf;
using UnityEngine;
#else
    System.Math;
#endif

namespace abc.App.Game.Code.Pure.UpgradableInt
{
    public class Multiplicative : IUpgradableInt
    {
        private Multiplicative.Runtime _runtime;
        private Gameplay.Runtime _gameplayRuntime;
        
        
#if ODIN_INSPECTOR
        [ShowInInspector]
        [ReadOnly]
#endif
        public int Value => (int)(StartingValue * Math.Pow( StepMultiplicator, Step));

#if ODIN_INSPECTOR
        [ShowInInspector] [ReadOnly]
#endif
        public int NextStepCost =>
            (int)(BaseCostPerStep *
                  Math.Pow(CostPreStepMultiplier, Step));
        

        private int _step; // save target
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

        public event Action<int> NewStep;
        void IUpgradableInt.AdvanceStep()
        {
            Step++;
        }
        public bool IsMaxed => (MaxStep > 0 && Step > MaxStep);

        [OdinSerialize] public int StepMultiplicator { get; protected set; }
        [OdinSerialize] public int StartingValue { get; protected set; }
        [OdinSerialize] public int MaxStep { get; protected set; }
        [OdinSerialize] public int BaseCostPerStep { get; protected set; }
        [OdinSerialize] public float CostPreStepMultiplier { get; protected set; }
        [OdinSerialize] public Resource.Type CostResourceType { get; protected set; }

        public interface IType { }
        
        [Serializable]
        public class Runtime : IIntProperty.Runtime
        {

            public Runtime(Gameplay.Runtime gameplayRuntime, Multiplicative instance, IPropertyType type) 
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