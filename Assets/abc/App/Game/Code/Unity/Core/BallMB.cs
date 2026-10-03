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

        internal void BindToCombo(GameplayMB gameplayMB, Pure.Ball ball)
        {
            contextBindField =  gameplayMB;
            Pure = ball;
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
                    if (ball == Pure && ctx == Pure.Context)
                        RandomizeBounce();
                })
                .AddTo(_tails);
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
            var surpriseJitter = Random
                .Range(-GameplayState.BallsJitter, GameplayState.BallsJitter);
            // Debug.Log($"old velocity: {collision.rigidbody.linearVelocity}, " +
            //           $"new velocity: {Quaternion.Euler(0,0,surpriseJitter) * collision.rigidbody.linearVelocity}");
            PhysicsBody.linearVelocity
                = Quaternion.Euler(0, 0, surpriseJitter)
                  * PhysicsBody.linearVelocity;
        }
        
        private void MaintainSpeed()
        {
            var targetSpeed = Pure.Speed;
            if (PhysicsBody.linearVelocity.magnitude == 0)
                PhysicsBody.linearVelocity = Random.insideUnitCircle;
            
            PhysicsBody.linearVelocity =
                PhysicsBody.linearVelocity.normalized * targetSpeed;
        }

        protected override void ClearBinds()
        {
            ContextBind?.Balls?.Remove(this);
            _tails?.Clear();
        }
    }
}