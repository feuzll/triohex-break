
using System;
using System.Collections.Generic;
using R3;
using Sirenix.Serialization;


namespace abc.App.Game.Code.Pure.Bosses
{
    [Serializable]
    public partial class Boss
    {
        [OdinSerialize] public string Name { get; private set; }

        internal Runtime Driver { get; private set; }
        
        protected ReactiveProperty<int> HealthProperty { get; set; }
        public ReadOnlyReactiveProperty<int> Health => HealthProperty;
        
        [OdinSerialize] public List<(string, float)> PeriodicCasts { get; private set; }
        [OdinSerialize] public List<string> Passives { get; private set; }
        
        public Observable<Unit> Defeated { get; private set; }
    }
}