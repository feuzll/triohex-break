using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using Math =
#if UNITY_5_3_OR_NEWER
    UnityEngine.Mathf;
#else
    System.Math;
#endif

namespace abc.App.Game.Code.Pure.UpgradableInt
{
    [Serializable]
    public class Incremental : IIntProperty
    {
        private Runtime _runtime;
        private Gameplay.Runtime _gameplayRuntime;
        
        
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
        public class Runtime : IIntProperty.Runtime
        {

            public Runtime(Gameplay.Runtime gameplayRuntime, Incremental instance, IPropertyType type) 
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
            }
            
            internal bool TryBuyNextStep()
            {
                Debug.Log("trying to buy next step");
                var Instance = this.Instance as Incremental;
                //guard
                if (Instance.MaxStep > 0 && Instance.Step >= Instance.MaxStep )
                    return false;
                var requiredResource = 
                    Instance._gameplayRuntime.Context.Resources[Instance.CostResourceType];
                var nextStepCost = Instance.NextStepCost;
                if (requiredResource.Amount.CurrentValue < nextStepCost)
                    return false;
                
                //actual buy
                requiredResource.AmountProperty.Value -= nextStepCost;
                Instance.Step++;
                Debug.Log("successfully bought next step");
                return true;
            }
            
        }
        
    }
}