using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Math =
#if UNITY_5_3_OR_NEWER
    UnityEngine.Mathf;
#else
    System.Math;
#endif

namespace abc.App.Game.Code.Pure.Upgrades
{
    [Serializable]
    public class Incremental
    {
        private Runtime _runtime;
        private Gameplay.Runtime _gameplayRuntime;
        
#if ODIN_INSPECTOR
        [ReadOnly]
#endif
        [OdinSerialize] private IType _type;
        
#if ODIN_INSPECTOR
        [ShowInInspector]
        [ReadOnly]
#endif
        public int Value => StartingValue + StepIncrement * Step;

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
        
        [OdinSerialize] public int StepIncrement { get; protected set; }
        [OdinSerialize] public int StartingValue { get; protected set; }
        [OdinSerialize] public int MaxStep { get; protected set; }
        [OdinSerialize] public int BaseCostPerStep { get; protected set; }
        [OdinSerialize] public float CostPreStepMultiplier { get; protected set; }
        [OdinSerialize] public Resource.Type CostResourceType { get; protected set; }

        public interface IType { }
        
        [Serializable]
        public class Runtime
        {
            [OdinSerialize] public readonly Incremental Instance;

            internal Runtime(Gameplay.Runtime gameplayRuntime, Incremental instance, IType type)
            {
                if (instance._runtime != null || instance._gameplayRuntime != null) 
                    throw new NotSupportedException();
                
                Instance = instance;
                Instance._type = type;
                
                Instance._runtime = this;
                Instance._gameplayRuntime = gameplayRuntime;
                
                if (Instance.StartingValue <= 0)
                    Instance.StartingValue = 1;
                
                if (Instance.MaxStep > 0 && Instance.Step > Instance.MaxStep)
                    Instance.Step = Instance.MaxStep;
                
                Instance.Step = 0;
                Instance._gameplayRuntime.Context.UpgradesRuntimes.Add(type, this);
            }
            
            internal void TryBuyNextStep()
            {
                //guard
                if (Instance.Step >= Instance.MaxStep )
                    return;
                var requiredResource = 
                    Instance._gameplayRuntime.Context.Resources[Instance.CostResourceType];
                var nextStepCost = Instance.NextStepCost;
                if (requiredResource.Amount.CurrentValue < nextStepCost)
                    return;
                
                //actual buy
                requiredResource.AmountProperty.Value -= nextStepCost;
                Instance.Step++;
            }
            
        }
        
    }
}