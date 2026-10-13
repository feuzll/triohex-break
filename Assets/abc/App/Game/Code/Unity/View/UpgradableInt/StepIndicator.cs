using System;
using abc.App.Game.Code.Pure.UpgradableInt;
using abc.App.Game.Code.Unity.Core;
using abc.App.Game.Code.Unity.Core.UpgradableInt;
using Sirenix.OdinInspector;
using UnityEngine;

namespace abc.App.Game.Code.Unity.View.UpgradableInt
{
    public class StepIndicator : SerializedMonoBehaviour
    {
        [field:SerializeField]
        public IncrementalType PropertyType { get; private set; }
        
        [SerializeField] private TMPro.TextMeshProUGUI stepIndicator;
        [SerializeField] private GameplayMB gameBind;
        
        private IUpgradableInt _upgradableInt;
        
        private void Start()
        {
            if (gameBind.State.IntProperties[PropertyType].Instance is not IUpgradableInt incremental)
                return;
            _upgradableInt =  incremental;
            Redraw(_upgradableInt.Value);
            _upgradableInt.NewStep += Redraw;
        }

        private void OnDestroy()
        {
            _upgradableInt.NewStep -= Redraw;
        }


        private void Redraw(int newValue)
        {
            stepIndicator.text = "x" + newValue;
        }
    }
}