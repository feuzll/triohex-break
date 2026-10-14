using System;
using System.Collections.Generic;
using ObservableCollections;
using R3;
#if UNITY_5_3_OR_NEWER
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
#endif

namespace abc.App.Game.Code.Pure.Bosses
{
    [Serializable]
    public partial class Rush
    {
        [OdinSerialize] protected List<IConceptScope<Boss>> PremadeBossesCycle { get; private set; }
        public List<Boss.Runtime> BossesCycle { get; private set; }
#if UNITY_5_3_OR_NEWER
        [InlineProperty]
        [SerializeField] internal SerializableReactiveProperty<int> LevelProperty;
#else
        internal ReactiveProperty<int> LevelProperty { get; set; }
#endif 
        public ReadOnlyReactiveProperty<int> Level => LevelProperty;
        
        #if ODIN_INSPECTOR
        [ShowInInspector] [ReadOnly]
        #endif
        internal ObservableQueue<(Boss.Runtime, int)> PendingFights { get; private set; }
        
        #if ODIN_INSPECTOR
        [ShowInInspector] [ReadOnly]
        #endif
        internal R3.ReactiveProperty<bool> IsFightingProperty;
        public ReadOnlyReactiveProperty<bool> IsFighting => IsFightingProperty;
        
        internal CompositeDisposable DefeatSubscriptions { get; private set; }
        internal IDisposable RewardSubscription { get; set; }
    }
}