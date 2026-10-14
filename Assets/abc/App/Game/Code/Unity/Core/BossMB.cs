using System;
using abc.App.Game.Code.Pure;
using abc.App.Game.Code.Pure.Bosses;
using R3;
using R3.Triggers;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace abc.App.Game.Code.Unity.Core
{
    public class BossMB : SerializedMonoBehaviour, IConceptScope<Boss>
    {
        private Observable<Collision2D> _onCollisionEnter2D;
        private CompositeDisposable _tails;
        private Observable<Unit> _onPureClear;

        [OdinSerialize] public Boss Pure { get; protected set; }
        
        private void Awake()
        {
            _tails = new();
            _onCollisionEnter2D = gameObject.OnCollisionEnter2DAsObservable();
            _onCollisionEnter2D
                .Select(c => c.collider.GetComponent<BallMB>())
                .Where(ballMB => ballMB != null)
                .Subscribe(ballMB => Code.Pure.Interaction.BallHitsBoss.Raise(Pure.Driver, ballMB.Pure))
                .AddTo(_tails);
        }

        private void Start()
        {
            Pure.Health.Subscribe(health => gameObject.SetActive(health > 0)).AddTo(_tails);
        }

        protected void OnDestroy()
        {
            _tails?.Clear();
        }

        Observable<Unit> IConceptScope<Boss>.OnPureClear => _onPureClear;
    }
}