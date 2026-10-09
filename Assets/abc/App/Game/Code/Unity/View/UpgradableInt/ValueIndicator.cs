using abc.App.Game.Code.Pure.UpgradableInt;
using abc.App.Game.Code.Unity.Core;
using abc.App.Game.Code.Unity.Core.UpgradableInt;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace abc.App.Game.Code.Unity.View.UpgradableInt
{
    public class ValueIndicator : SerializedMonoBehaviour
    {
        [field:SerializeField]
        public IncrementalType PropertyType { get; private set; }
        
        [FormerlySerializedAs("stepIndicator")] [SerializeField] private TMPro.TextMeshProUGUI valueIndicator;
        [SerializeField] private GameplayMB gameBind;
        
        private void Start()
        {
            var incremental = gameBind.State.IntProperties[PropertyType].Instance as Incremental;
            valueIndicator.text = "- " + incremental.Value.ToString() + " -";
            incremental.NewStep += _ =>
            {
                valueIndicator.text = "- " + incremental.Value.ToString() + " -";
            };
        }
    }
}