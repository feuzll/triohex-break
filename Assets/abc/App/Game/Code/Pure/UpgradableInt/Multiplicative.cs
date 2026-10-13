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
    [Serializable]
    public class Multiplicative : Base, IUpgradableInt
    {
#if ODIN_INSPECTOR
        [ShowInInspector]
        [ReadOnly]
#endif
        public override int Value => (int)(StartingValue * Math.Pow( StepMultiplicator, Step));

        [OdinSerialize] public int StepMultiplicator { get; protected set; }
    }
}