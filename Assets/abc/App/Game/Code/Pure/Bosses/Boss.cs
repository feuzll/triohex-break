
using System;
using System.Collections.Generic;
using R3;
using Sirenix.Serialization;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace abc.App.Game.Code.Pure.Bosses
{
    [Serializable]
    public partial class Boss
    {
        [OdinSerialize] public string Name { get; private set; }

        internal Runtime Driver { get; private set; }
        
        #if ODIN_INSPECTOR
        [ShowInInspector] [ReadOnly]
        #endif
        protected ReactiveProperty<int> HealthProperty { get; set; }
        public ReadOnlyReactiveProperty<int> Health => HealthProperty;
        
        //templates for future effects...
        [OdinSerialize] public List<(string, float)> PeriodicCasts { get; private set; }
        [OdinSerialize] public List<string> Passives { get; private set; }
        
        public Observable<Unit> Defeated { get; private set; }
    }
}