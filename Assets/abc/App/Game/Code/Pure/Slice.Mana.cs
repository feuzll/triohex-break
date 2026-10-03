using System;
using R3;
using Sirenix.Serialization;

#if UNITY_5_3_OR_NEWER
using Sirenix.OdinInspector;
using UnityEngine;
#endif

namespace abc.App.Game.Code.Pure
{
    public partial class Slice
    {
        [Serializable]
        public partial class Mana
        {
#if UNITY_5_3_OR_NEWER
            [InlineProperty] [SerializeField] 
            internal SerializableReactiveProperty<float> AmountProperty;
#else
            internal ReactiveProperty<float> AmountProperty { get; }
#endif
            public ReadOnlyReactiveProperty<float> Amount => AmountProperty;

            
#if UNITY_5_3_OR_NEWER
            [InlineProperty] [SerializeField] 
            internal SerializableReactiveProperty<float> LimitProperty;
#else
            internal ReactiveProperty<float> LimitProperty { get; }
#endif 
            public ReadOnlyReactiveProperty<float> Limit => LimitProperty;
            
            #if ODIN_INSPECTOR
            [MinValue(0f)] [MaxValue(1f)]
            #endif
            [OdinSerialize] public float ManaRegenPercentPerSecond { get;  protected set; }
            
            [OdinSerialize] public float CostPerUnit { get;  protected set; }
                
            [OdinSerialize] public float MinToStart { get;  protected set; }
        }
    }
}