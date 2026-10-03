using abc.App.Game.Code.Unity.Core;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;

namespace abc.App.Game.Code.Unity.View
{
    
    public sealed class SliceTrail : SerializedMonoBehaviour
    {
        [SerializeField] TrailRenderer trail;
        [SerializeField] private GameplayMB gameplayBind;
        [SerializeField] private float zOffset = -1f;
        
        private void Awake()
        {
            trail.emitting = false;
        }

        private void Start()
        {
            gameplayBind.State.Tick.Subscribe(_ =>
                trail.transform.position = 
                    (Vector3)gameplayBind.State.PointerPosition 
                    + Vector3.forward * zOffset);
            gameplayBind.State.Slice.SwipeStarted += () => trail.emitting = true;
            gameplayBind.State.Slice.SwipeEnded += () => trail.emitting = false;
        }
        
        //TODO: OnDestroy unbind
    }
}