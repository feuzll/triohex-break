using System;
using System.Linq;
using R3;
using R3.Triggers;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace abc.App.Game.Code.Unity.Core
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Collider2D))]
    public class CellMB : MonoBind<Pure.Cell>
    {
        internal Collider2D Collider => _collider ??= GetComponent<Collider2D>();
        private Collider2D _collider;
        
        internal SpriteRenderer SpriteRenderer => _spriteRenderer ??= GetComponent<SpriteRenderer>();
        private SpriteRenderer _spriteRenderer;
        
        [ShowInInspector] [OdinSerialize]
        public Pure.Cell.Type Variation
        {
            get => _type;
            private set //manual inspector type change
            {
                _type = value;
                if (ContextBind != null)
                    Pure = new Pure.Cell(Context, _type, 
                        this, ActiveOnGameplayStart);
            }
        }
        private Pure.Cell.Type _type = Code.Pure.Cell.Type.Basic;

        private bool ActiveOnGameplayStart =>
            ContextBind.MonoFieldSchemesCycle[0]
                .CellsAsObstacles.Contains(this);
        
        [DisableInEditorMode]
        internal GameplayMB ContextBind
        {
            get => contextBindField;
            set //runtime manual inspector change
            {
                if (Pure != null) ClearBinds();
                contextBindField = value;
                Pure = value == null ?
                    null : new Pure.Cell(value.State, Variation, 
                        this, ActiveOnGameplayStart);
            }
        }
        [SerializeField] [ReadOnly] private GameplayMB contextBindField;
        private Pure.Gameplay Context =>  ContextBind?.State;
        
        
        private CompositeDisposable  _tails;

        private void Awake()
        {
            gameObject.OnCollisionEnter2DAsObservable()
                .Select(c =>
                {
                    (Pure.Gameplay ctx, Pure.Cell cell, Pure.Ball ball) e;
                    e.ctx = Context;
                    e.cell = Pure;
                    e.ball = c.collider.GetComponent<BallMB>().Pure;
                    return e;
                })
                .Subscribe(e => 
                    Code.Pure.Interaction.BallHitsCell
                        .Raise(e.ctx, e.cell, e.ball)).AddTo(gameObject);
        }

        protected override void OnNewPureState()
        {
            _tails ??= new CompositeDisposable();

            if (Pure != null) ContextBind?.MonoFieldCells.Add(this);
            
            Pure?.Health.Subscribe(_ =>
            {
                gameObject.SetActive(Pure.Health.CurrentValue > 0);
            }).AddTo(_tails);

            Pure?.IsArmored.Subscribe(isArmored =>
            {
                SpriteRenderer.sprite = isArmored ?
                    ContextBind.armoredCellSprite : ContextBind.basicCellSprite;
            });
        }

        protected override void ClearBinds()
        {
            ContextBind?.MonoFieldCells.Remove(this);
            _tails?.Clear();
        }
    }
}