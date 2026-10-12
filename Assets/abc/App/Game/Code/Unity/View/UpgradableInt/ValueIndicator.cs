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
            if (gameBind.State.IntProperties[PropertyType].Instance is Incremental)
            {
                var incremental = gameBind.State.IntProperties[PropertyType].Instance as Incremental;
                valueIndicator.text = "- " + incremental.Value.ToString() + " -";
                incremental.NewStep += _ =>
                {
                    valueIndicator.text = "- " + incremental.Value.ToString() + " -";
                };
            }
            else if (gameBind.State.IntProperties[PropertyType].Instance is Multiplicative)
            {
                var multiplicative = gameBind.State.IntProperties[PropertyType].Instance as Multiplicative;
                valueIndicator.text = "- " + multiplicative.Value.ToString() + " -";
                multiplicative.NewStep += _ =>
                {
                    valueIndicator.text = "- " + multiplicative.Value.ToString() + " -";
                };
            }
        }
    }
}