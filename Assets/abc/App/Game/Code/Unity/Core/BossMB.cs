using System;
using abc.App.Game.Code.Pure.Bosses;
using R3;
using R3.Triggers;
using UnityEngine;

namespace abc.App.Game.Code.Unity.Core
{
    public class BossMB : MonoBehaviour
    {
        private Observable<Collision2D> _onCollisionEnter2D;
        private CompositeDisposable _tails;

        [field:SerializeField] public Boss Instance { get; protected set; }
        
        private void Awake()
        {
            _onCollisionEnter2D = gameObject.OnCollisionEnter2DAsObservable();
            _onCollisionEnter2D
                .Select(c => c.collider.GetComponent<BallMB>())
                .Where(ballMB => ballMB != null)
                .Subscribe(ballMB => Code.Pure.Interaction.BallHitsBoss.Raise(Instance.Driver, ballMB.Pure))
                .AddTo(_tails);
        }

        protected void OnDestroy()
        {
            _tails?.Clear();
        }
    }
}