using System;
using abc.App.Game.Code.Pure;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;

namespace abc.App.Game.Code.Unity.Core
{
    public abstract class MonoBind<T> : SerializedMonoBehaviour, IConceptScope<T>
    {
        private Subject<Unit> _onPureClear = new();
        Observable<Unit> IConceptScope<T>.OnPureClear =>
            _onPureClear ??= new();

        [ReadOnly]
        public virtual T Pure
        {
            get => _pureField;
            // reaction to context\type change
            protected set
            {
                if (Pure != null)
                {
                    ClearBinds();
                    _onPureClear.OnNext(Unit.Default);
                }

                _pureField = value;
                OnNewPureState();
            }
            
        }

        
        protected abstract void OnNewPureState();
        protected abstract void ClearBinds();
        private T _pureField;

        protected virtual void OnValidate()
        {
            if (!Application.isPlaying)
                ClearBinds();
        }
        
        protected virtual void OnDestroy()
        {
            ClearBinds();
            _onPureClear.OnNext(Unit.Default);
            _onPureClear.OnCompleted();
            _onPureClear.Dispose();
        }
    }
}