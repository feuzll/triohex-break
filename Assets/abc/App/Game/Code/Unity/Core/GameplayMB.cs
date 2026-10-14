using System.Collections.Generic;
using System.Linq;
using abc.App.Game.Code.Pure;
using abc.App.Game.Code.Pure.Bosses;
using abc.App.Game.Code.Pure.Interaction;
using abc.App.Game.Code.Pure.UpgradableInt;
using abc.App.Game.Code.Unity.Core.UpgradableInt;
using R3;
using R3.Triggers;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace abc.App.Game.Code.Unity.Core
{
    
    public class GameplayMB : SerializedMonoBehaviour
    {
        [OdinSerialize] public Pure.Gameplay State {get; private set;}
        private Gameplay.Runtime _runtime;
        

        [TitleGroup("Model-Unity")]
        [SerializeField] internal HashSet<CellMB> MonoFieldCells;
        
        [TitleGroup("Model-Unity")]
        [SerializeField] internal List<FieldSchemeMB> MonoFieldSchemesCycle;
        
        [TitleGroup("Model-Unity")]
        [SerializeField] private EdgeCollider2D FieldBorder;
        
        [TitleGroup("Model-Unity")]
        [field: SerializeField] internal Camera Camera { get; private set; }

        [OdinSerialize] private Dictionary<Pure.Ball.Type, BallMB> BallPrefabs;

        [OdinSerialize] [DisableInPlayMode]
        internal HashSet<BallMB> Balls;
        
        [SerializeField] LayerMask sliceCellMask;
        internal ContactFilter2D CellSearchFilter;
        private readonly RaycastHit2D[] SliceHits = new RaycastHit2D[32];
        private readonly Collider2D[] SliceOverlaps = new Collider2D[8];
        
        internal Observable<Unit> FixedTick { get; private set; }

        [SerializeField] private HashSet<UpgradableInt.IPurchase> _upgradeSources;
        [SerializeField] private HashSet<IBallPurchase> _ballPurchaseSources;
        [SerializeField] private ObjectPool<BallMB> _oneHitBallsPool;
        [SerializeField] private Button prestigeButton;

        [SerializeField] internal Sprite basicCellSprite;
        [SerializeField] internal Sprite armoredCellSprite;
        [SerializeField] internal Button bossRushEnter;
        [SerializeField] internal Button bossRushExit;
        
        private void Awake()
        {
            foreach (var ballPurchase in _ballPurchaseSources)
            {
                ballPurchase.BuyButton.onClick.AddListener(() =>
                {
                    var success =
                    Pure.Interaction.BallPurchase
                        .Try(State, ballPurchase.BallType,
                            () =>
                            {
                                var newBallBind = Instantiate(BallPrefabs[ballPurchase.BallType]);
                                Balls.Add(newBallBind);
                                return newBallBind;
                            }, out var newBall);
                    if (success)  ((BallMB)newBall.Scope)
                        .BindToCombo(this, newBall);
                });
            }

            foreach (var purchase in _upgradeSources)
            {
                purchase.BuyButton.onClick.AddListener((() =>
                {
                    var success = Pure.Interaction.UpgradeIntProperty.Try(State, purchase.PropertyType);
                }));
            }
            
            CellSearchFilter = new ContactFilter2D 
                { useLayerMask = true, layerMask = sliceCellMask, useTriggers = true };
            
            FixedTick = gameObject.FixedUpdateAsObservable();
            _runtime = new Code.Pure.Gameplay.Runtime(State, 
                gameObject.UpdateAsObservable().Select(dt => Time.deltaTime));
            
            
            State.FieldCollision = FieldBorder.OnCollisionEnter2DAsObservable()
                .Select(c => c.collider.GetComponent<BallMB>().Pure);


            _oneHitBallsPool = new (
                createFunc: () =>
                {
                    BallPrefabs[Ball.Type.OneHit].gameObject.SetActive(false);
                    var newBallBind = Instantiate(BallPrefabs[Ball.Type.OneHit]);
                    newBallBind.gameObject.SetActive(false);
                    Pure.Interaction.BallHitsCell.New.Subscribe(info =>
                    {
                        if (info.Item1 == State && info.Item3 == newBallBind.Pure)
                            _oneHitBallsPool.Release(newBallBind);
                    }).AddTo(newBallBind);
                    return newBallBind;
                },
                actionOnGet: null,
                actionOnRelease: mb =>
                {
                    mb.ContextBind = null;
                    mb.gameObject.SetActive(false);
                },
                actionOnDestroy: null,
                collectionCheck: true,
                defaultCapacity: 20,
                maxSize: 20000
                );
            
            
            Pure.Interaction.SplitterBallHitsWall.New.Subscribe(info =>
            {
                if (info.Item1 != State) return;
                for (var i = 0; i < State.IntProperties[State.SplitterBallDivideCountType].Instance.Value; i++)
                {
                    var parentSplitterBall = (BallMB)info.Item2.Scope; 
                    var newOneHitBallVelocity = parentSplitterBall.PhysicsBody.linearVelocity;
                    var newOneHitBallPosition = parentSplitterBall.transform.position;
                    var newOneHitBallScope = _oneHitBallsPool.Get();
                    newOneHitBallScope.BindToCombo(this,
                        new Ball(State, Ball.Type.OneHit, newOneHitBallScope),
                        newOneHitBallVelocity, newOneHitBallPosition);
                    newOneHitBallScope.gameObject.SetActive(true);
                }
            }).AddTo(this);
            
            foreach (var (ballType,ballUpgradeType) in State.BallsSpeedTypes)
            {
                switch (ballUpgradeType)
                {
                    case IncrementalType incrementalType:
                        new Incremental.Runtime(_runtime,
                            Instantiate(incrementalType).runtimeTemplate, ballUpgradeType);
                        break;
                    case ConstantInt constantInt:
                        new IIntProperty.Runtime(_runtime, Instantiate(constantInt), constantInt);
                        break;
                }
            }

            new Multiplicative.Runtime(_runtime,
                Instantiate((MultiplicativeType)State.AnyBallPowerMultiplicatorType).runtimeTemplate,
                State.AnyBallPowerMultiplicatorType);
            
            new Incremental.Runtime(_runtime,
                Instantiate(((IncrementalType)State.AnyBallAcceleratorType)).runtimeTemplate,
                State.AnyBallAcceleratorType);
            
            new Incremental.Runtime(_runtime,
                Instantiate(((IncrementalType)State.AOEBallRangeType)).runtimeTemplate,
                State.AOEBallRangeType); 
            
            new Incremental.Runtime(_runtime,
                Instantiate(((IncrementalType)State.SplitterBallDivideCountType)).runtimeTemplate,
                State.SplitterBallDivideCountType); 

            foreach (var (ballType,ballUpgradeType) in State.BallsPowersTypes)
                new Incremental.Runtime(_runtime,
                    Instantiate((IncrementalType)ballUpgradeType )
                        .runtimeTemplate, ballUpgradeType);
            
            foreach (var (ballType,ballUpgradeType) in State.BallCostsTypes)
                new Incremental.Runtime(_runtime,
                    Instantiate((IncrementalType)ballUpgradeType)
                        .runtimeTemplate, ballUpgradeType);
            
            prestigeButton.onClick.AddListener(() =>
            {
                if (State.PendingPrestigePoints.CurrentValue <= 0) return;
                State.Resources[Resource.Type.Prestige].AmountProperty.Value += 
                    State.PendingPrestigePoints.CurrentValue;
                foreach (var ballMB in Balls) Destroy(ballMB.gameObject);
                State.levelProperty.Value = 1;
            });
            
            
            
            bossRushEnter.onClick.AddListener(() =>
                PlayerStartsBossRush.Try(_runtime, State.BossRushRuntime));
            bossRushEnter.onClick.AddListener(() =>
                PlayerExitsBossRush.Raise(_runtime, State.BossRushRuntime));

            this.State.Tick.Subscribe(_ =>
                State.PointerPosition =
                    Camera.ScreenToWorldPoint(Input.mousePosition));
            
            this.State.Tick.Subscribe(_ =>
                State.PointerJustPressed =
                    Input.GetMouseButtonDown(0));
            
            this.State.Tick.Subscribe(_ =>
                State.PointerPressed =
                    Input.GetMouseButton(0));
            
            this.State.Tick.Subscribe(_ =>
                State.PointerJustReleased =
                    Input.GetMouseButtonUp(0));
            
            
            foreach (var cellMB in MonoFieldCells.ToArray()) 
                cellMB.ContextBind = this;
            
            foreach (var fieldSchemeMB in MonoFieldSchemesCycle) 
                fieldSchemeMB.ContextBind = this;
            State.ManaRuntime = new Pure.Slice.Mana.Runtime(State);
            State.SliceRuntime = new Pure.Slice.Runtime(State, CollectCellsOnMotion);
        }

        protected Pure.Cell[] CollectCellsOnMotion(Vector2 a, Vector2 b)
        {
            Physics2D.SyncTransforms();   // only needed if cells move via Transform; see notes
            
            var d = b - a;
            var unityD = b -  a;
                
            float dist = d.magnitude;
            int n;

            if (dist < 1e-4f)             // press with no movement
            {
                n = Physics2D.OverlapCircle(a, State.Slice.BladeRadius,
                    CellSearchFilter, SliceOverlaps);
                var overlaps = SliceOverlaps.Take(n);
                return MonoFieldCells
                    .Where(cell => overlaps.Contains(cell.Collider))
                    .Select(c => c.Pure).ToArray();
            }

            n = State.Slice.BladeRadius > 0f
                ? Physics2D.CircleCast(a, State.Slice.BladeRadius,
                    d / dist, CellSearchFilter, SliceHits, dist)
                : Physics2D.Linecast(a, b, CellSearchFilter, SliceHits);

            //Debug.Log($"slice hit count: {n}");
            
            var hitColliders = 
                SliceHits.Take(n).Select(h => h.collider); 
                
            return MonoFieldCells.Where(cell => 
                hitColliders.Contains(cell.Collider))
                .Select(c=> c.Pure).ToArray();
        }

        [Button]
        private void PremadeCellsFromHierarchy(Transform hierarchy)
        {
            MonoFieldCells.Clear();
            foreach (var cellMB in hierarchy
                         .GetComponentsInChildren<CellMB>(true))
                MonoFieldCells.Add(cellMB);
        }
        
        private void OnValidate()
        {
            if (Application.isPlaying) return;
            State.Validate();
            if (State.Level == null) return;
            if (State.levelProperty.Value <= 0) State.levelProperty.Value = 1; 
            MonoFieldSchemesCycle = MonoFieldSchemesCycle?.Distinct().ToList();
            var obstacles = MonoFieldSchemesCycle?[
                (State.Level.CurrentValue - 1)%MonoFieldSchemesCycle.Count].CellsAsObstacles;
            State.BreaksToNextLevel.Value = obstacles is { Count: > 0 } ? obstacles.Count : 1;
        }
    }
}