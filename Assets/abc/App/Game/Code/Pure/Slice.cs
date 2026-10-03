using System;
using System.Collections.Generic;
using System.Numerics;
using R3;
using Sirenix.Serialization;

namespace abc.App.Game.Code.Pure
{
    [Serializable]
    public partial class Slice : ISlice
    {
        protected internal Slice() {} //only serialized
        
        public event Action<Cell> CellSliced;
        public event Action SwipeStarted;
        public event Action SwipeEnded;        // released OR ran out of mana

        [OdinSerialize] public float BladeRadius { get; private set; } = 0.05f;
        [OdinSerialize] public float MinSegment { get; private set; } = 0.01f;
        
        [OdinSerialize] public float HitCooldown { get; private set; } = 0.2f;
        [OdinSerialize] public int Strength { get; private set; } = 1;
        
    }
}