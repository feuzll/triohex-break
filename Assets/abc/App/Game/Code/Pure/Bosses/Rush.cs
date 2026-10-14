using System;
using System.Collections.Generic;
using ObservableCollections;
using R3;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace abc.App.Game.Code.Pure.Bosses
{
    [Serializable]
    public partial class Rush
    {
        [OdinSerialize] protected List<Boss> PremadeBossesCycle { get; private set; }
        public List<Boss.Runtime> BossesCycle { get; private set; }
#if UNITY_5_3_OR_NEWER
        [InlineProperty]
        [SerializeField] internal SerializableReactiveProperty<int> LevelProperty;
#else
        internal ReactiveProperty<int> LevelProperty { get; set; }
#endif 
        public ReadOnlyReactiveProperty<int> Level => LevelProperty;
        
        internal ObservableStack<(Boss.Runtime, int)> PendingFights { get; private set; }
        
        internal readonly R3.ReactiveProperty<bool> IsFightingProperty = new(false);
        public ReadOnlyReactiveProperty<bool> IsFighting => IsFightingProperty;
        
        internal CompositeDisposable DefeatSubscriptions { get; } = new CompositeDisposable();
    }
}