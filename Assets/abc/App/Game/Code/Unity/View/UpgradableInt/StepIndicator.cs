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
        
        private void Start()
        {
            var incremental = gameBind.State.IntProperties[PropertyType].Instance as Incremental;
            stepIndicator.text = "x" + incremental.Step.ToString();
            incremental.NewStep += step =>
            {
                stepIndicator.text = "x" + step.ToString();
            };
        }
    }
}