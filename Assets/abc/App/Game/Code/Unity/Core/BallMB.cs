using System.Collections.Generic;
using System.Linq;
using abc.App.Game.Code.Pure;
using R3;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using Random = UnityEngine.Random;

namespace abc.App.Game.Code.Unity.Core
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class BallMB : MonoBind<Pure.Ball>
    {
        [ShowInInspector] [ReadOnly]
        public Rigidbody2D PhysicsBody => 
            _physicsBody ??= GetComponent<Rigidbody2D>();
        private Rigidbody2D _physicsBody;
        
        
        private CompositeDisposable  _tails;

        internal void BindToCombo(GameplayMB gameplayMB, Pure.Ball ball, 
            Vector2? initialVelocity = null, Vector2? initialPosition = null)
        {
            contextBindField =  gameplayMB;
            Pure = ball;
            if (initialVelocity != null)
                PhysicsBody.linearVelocity = initialVelocity.Value;
            if (initialPosition != null)
                transform.position = initialPosition.Value;
        }

        protected override void OnNewPureState()
        {
            _tails ??= new CompositeDisposable();
            if (Pure == null)
            {
                contextBindField = null;
                return;
            }
            
            _type = Pure.Variation;
            ContextBind.Balls.Add(this);
                    
                    
            ContextBind.FixedTick
                .Subscribe(_ => MaintainSpeed())
                .AddTo(_tails);
                    
            GameplayState.FieldCollision
                .Where(b => b == Pure)
                .Subscribe(_ => RandomizeBounce())
                .AddTo(_tails);

            Code.Pure.Interaction
                .BallHitsCell.New
                .Subscribe((e) =>
                {
                    var (ctx, anyCell, ball) = e;
                    if (ball != Pure || ctx != Pure.Context)
                        return;
                    RandomizeBounce();
                    if (Variation == Ball.Type.AOE)
                        AOEBallCollision();
                        
                })
                .AddTo(_tails);

            GameplayState.FieldCollision.Subscribe((ball) =>
            {
                if (Pure == null) return;
                if (ball == Pure && ball.Variation == Ball.Type.Sniper)
                    RotateToClosestCellBindOrRandomly();
            }).AddTo(_tails);
            
            GameplayState.FieldCollision.Subscribe((ball) =>
            {
                if (Pure == null) return;
                if (ball == Pure && ball.Variation == Ball.Type.Splitter)
                    Code.Pure.Interaction.SplitterBallHitsWall.Raise(GameplayState, ball);
            }).AddTo(_tails);
        }
        
        
        [ShowInInspector] [OdinSerialize]
        public Pure.Ball.Type Variation
        {
            get => _type;
            private set //manual inspector type change
            {
                _type = value;
                if (ContextBind != null)
                    Pure = new Pure.Ball(GameplayState, _type, this);
            }
        }
        private Pure.Ball.Type _type = Code.Pure.Ball.Type.Basic;
        
        
        
        [DisableInEditorMode]
        internal GameplayMB ContextBind
        {
            get => contextBindField;
            set //runtime manual inspector change
            {
                if (Pure != null) ClearBinds();
                contextBindField = value;
                Pure = value == null ?
                    null : new Pure.Ball(value.State, _type, this);
            }
        }
        [SerializeField] [ReadOnly] private GameplayMB contextBindField;
        private Pure.Gameplay GameplayState =>  ContextBind?.State;

        private void RandomizeBounce()
        {
            //Debug.Log("Random bounce");
            var surpriseJitter = Random
                .Range(-GameplayState.BallsJitter, GameplayState.BallsJitter);
            // Debug.Log($"old velocity: {collision.rigidbody.linearVelocity}, " +
            //           $"new velocity: {Quaternion.Euler(0,0,surpriseJitter) * collision.rigidbody.linearVelocity}");
            PhysicsBody.linearVelocity
                = Quaternion.Euler(0, 0, surpriseJitter)
                  * PhysicsBody.linearVelocity;
        }

        private void RotateToClosestCellBindOrRandomly()
        {
            //Debug.Log("Sniper field border bounce trying...");
            var cellsBinds = 
                ContextBind.MonoFieldCells
                    .Where(cb => cb.isActiveAndEnabled).ToArray();
            if (cellsBinds.Length == 0)
            {
                RandomizeBounce();
                return;
            }
            CellMB closest = null;
            float bestSqr = float.MaxValue;
            foreach (var cellBind in cellsBinds)
            {
                float d = (cellBind.transform.position - transform.position).sqrMagnitude;
                if (d < bestSqr) { bestSqr = d; closest = cellBind; }
            }

            //closest.transform.localScale *= 1.1f;
            var newDir = closest.transform.position - transform.position;
            PhysicsBody.linearVelocity = newDir.normalized * Pure.Speed;
            //Debug.Log("Sniper field border bounce: success");
            //Debug.Log($"this frame vel: {PhysicsBody.linearVelocity}");
            // Observable.NextFrame()
            //     .Subscribe(_ => Debug.Log($"next frame vel: {PhysicsBody.linearVelocity}"));
        }

        private List<Collider2D> aoeBallCasts = new List<Collider2D>(10);
        private void AOEBallCollision()
        {
            Physics2D.SyncTransforms();
            var overlaps = Physics2D.OverlapCircle(transform.position,
                GameplayState.IntProperties[GameplayState.AOEBallRangeType].Instance.Value,
                ContextBind.CellSearchFilter, aoeBallCasts);
            for (var i = 0; i < overlaps; i++)
            {
                aoeBallCasts[i].TryGetComponent<CellMB>(out var cellMB);
                Code.Pure.Interaction.BallAOEOverlapsCell
                    .Raise(GameplayState, cellMB.Pure,  Mathf.Max(Pure.Strength/4, 1));
            }
        }
        
        private void MaintainSpeed()
        {
            if (PhysicsBody.linearVelocity.magnitude == 0)
                PhysicsBody.linearVelocity = Random.insideUnitCircle;
            
            PhysicsBody.linearVelocity =
                PhysicsBody.linearVelocity.normalized * Pure.Speed;
        }

        protected override void ClearBinds()
        {
            ContextBind?.Balls?.Remove(this);
            _tails?.Clear();
        }
    }
}