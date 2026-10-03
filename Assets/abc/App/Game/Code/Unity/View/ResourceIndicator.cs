using System;
using abc.App.Game.Code.Pure;
using abc.App.Game.Code.Unity.Core;
using R3;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace abc.App.Game.Code.Unity.View
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class ResourceIndicator : SerializedMonoBehaviour
    {
        [SerializeField] private GameplayMB _gameplay;
        [SerializeField] private Resource.Type resourceType;

        private TMPro.TextMeshProUGUI _indicator;
        private IDisposable _subscription = null;

        private void Awake()
        {
            _indicator = GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            //Debug.Log("indicator start");
            _subscription = _gameplay
                .State
                .Resources[resourceType]
                .Amount
                .Subscribe(_indicator, 
                    (amount, i) => 
                        i.text = amount.ToString());
        }

        private void OnDestroy() => _subscription?.Dispose();
    }
}