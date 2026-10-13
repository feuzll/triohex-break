using abc.App.Game.Code.Pure;
using abc.App.Game.Code.Pure.UpgradableInt;
using abc.App.Game.Code.Unity.Core;
using abc.App.Game.Code.Unity.Core.UpgradableInt;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

namespace abc.App.Game.Code.Unity.View.UpgradableInt
{
    public class ValueIndicator : SerializedMonoBehaviour
    {
        [OdinSerialize]
        public IPropertyType PropertyType { get; private set; }
        
        [FormerlySerializedAs("stepIndicator")] [SerializeField] private TMPro.TextMeshProUGUI valueIndicator;
        [SerializeField] private GameplayMB gameBind;
        
        private void Start()
        {
            if (gameBind.State.IntProperties[PropertyType].Instance is not IUpgradableInt) return;
            var upgradableInt = gameBind.State.IntProperties[PropertyType].Instance as IUpgradableInt;
            valueIndicator.text = "- " + upgradableInt.Value.ToString() + " -";
            upgradableInt.NewStep += _ =>
            {
                valueIndicator.text = "- " + upgradableInt.Value.ToString() + " -";
            };
        }
    }
}