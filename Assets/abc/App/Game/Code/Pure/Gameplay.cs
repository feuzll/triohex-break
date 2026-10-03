using System;
using System.Collections.Generic;
using abc.App.Game.Code.Pure.Upgrades;
using ObservableCollections;
using R3;
using Sirenix.Serialization;
using Vector2 =
    #if UNITY_5_3_OR_NEWER
    UnityEngine.Vector2;
    #else
    System.Numerics.Vector2;
#endif
#if UNITY_5_3_OR_NEWER
    #if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
    #endif
using UnityEngine;
#endif

namespace abc.App.Game.Code.Pure
{
    [Serializable]
    public partial class Gameplay
    {
        public interface IRules {}
        
#if ODIN_INSPECTOR
        [ShowInInspector] [ReadOnly] 
#endif
        public IRules Driver { get; private set; }
        
#if ODIN_INSPECTOR
        [ShowInInspector] [ReadOnly] [TitleGroup("Model-Pure")]
#endif
        public Dictionary<Pure.Resource.Type, Resource> Resources { get; protected set; } = new();
        
#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly, TitleGroup("Model-Pure")]
#endif
        public ObservableHashSet<Ball> Balls { get; protected set; } = new();
        
        public ObservableHashSet<Cell> RuntimePureCells { get; private set; }
        
        [OdinSerialize] public Slice Slice { get; internal set; }
        [OdinSerialize] public Slice.Mana Mana { get; internal set; }
        
        internal Slice.Runtime SliceRuntime { get; set; }
        internal Slice.Mana.Runtime ManaRuntime { get; set; }
        
#if ODIN_INSPECTOR
        [TitleGroup("Model-Pure")]
#endif
        [OdinSerialize]
        protected Dictionary<Pure.Resource.Type, int> StartingResources { get; private set; }
        
#if ODIN_INSPECTOR
        [TitleGroup("Model-Pure")]
#endif
        [OdinSerialize]
        internal Dictionary<Pure.Ball.Type, int> BallCosts { get; private set; }
      
      
#if ODIN_INSPECTOR
        [TitleGroup("Model-Pure")]
#endif
        [OdinSerialize] internal Dictionary<Pure.Ball.Type, Incremental.IType>
            BallsSpeedUpgradesTypes { get; set; }
        
      
#if ODIN_INSPECTOR
        [TitleGroup("Model-Pure")]
#endif
        [OdinSerialize] internal Dictionary<Pure.Ball.Type, Incremental.IType>
            BallsPowersUpgradesTypes { get; set; }
        
        
#if ODIN_INSPECTOR
        [TitleGroup("Model-Pure")]
#endif
        [OdinSerialize]
        public float BallsJitter { get; private set; } = 30;
        
        [OdinSerialize]
        public Incremental.IType AOEBallRangeUpgradeType { get; private set; } 
        
#if UNITY_5_3_OR_NEWER
        [DisableInPlayMode] [InlineProperty]
        [SerializeField] internal SerializableReactiveProperty<int> levelProperty;
#else
        internal ReactiveProperty<int> levelProperty { get; set; }
#endif 
        public ReadOnlyReactiveProperty<int> Level => levelProperty; 

        internal List<FieldScheme> FieldSchemesCycle { get; private set; } = new List<FieldScheme>();
        
        
#if UNITY_5_3_OR_NEWER
        [DisableInPlayMode] [InlineProperty] [ReadOnly]
        [SerializeField] internal SerializableReactiveProperty<int> breaksToNextLevelProperty;
#else
        internal ReactiveProperty<int> breaksToNextLevelProperty { get; set; }
#endif
        internal ReactiveProperty<int> BreaksToNextLevel => breaksToNextLevelProperty;
        
        protected Observable<Unit> BrokeAllFieldCells { get; private set; }
        
        protected bool TryBuyBall(Ball.Type type, Func<Ball.Type,Ball> spawnBallCallback)
        {
            var basicResource = Resources[Resource.Type.Basic];
            if (basicResource.Amount.CurrentValue < BallCosts[type]) return false;
            basicResource.AmountProperty.Value -= BallCosts[type];
            Balls.Add(spawnBallCallback(type));
            return true;
        }
        
        public Observable<float> Tick { get; internal set; }
        internal Observable<Ball> FieldCollision { get; set; }
        
        internal bool PointerJustPressed { get; set; }
        internal bool PointerPressed { get; set; }
        internal bool PointerJustReleased { get; set; }
        
        public Vector2 PointerPosition { get; internal set; }
        internal float Time { get; private set; }
        
#if ODIN_INSPECTOR
        [TitleGroup("Model-Pure")]
        // no add / remove / reorder, elements still editable
        [ListDrawerSettings(IsReadOnly = true)]
        [DisableInEditorMode]
        [HideReferenceObjectPicker]  
#endif
        [OdinSerialize] public Dictionary<Incremental.IType, Incremental.Runtime> UpgradesRuntimes { get; internal set; }
        
        internal void Validate()
        {
            if (levelProperty.Value <= 0) levelProperty.Value = 1;
        }
    }
    
}