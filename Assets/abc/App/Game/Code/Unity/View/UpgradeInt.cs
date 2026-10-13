using System;
using abc.App.Game.Code.Pure;
using abc.App.Game.Code.Pure.UpgradableInt;
using abc.App.Game.Code.Unity.Core;
using R3;
using Sirenix.OdinInspector;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UI;

namespace abc.App.Game.Code.Unity.View
{
    [RequireComponent(typeof(Button))]
    public class UpgradeInt : SerializedMonoBehaviour, Core.UpgradableInt.IPurchase
    {
        private Button _buyButton;
        
        [ShowInInspector] [ReadOnly]
        public Button BuyButton => 
            _buyButton ??= GetComponent<Button>();

        [field:SerializeField]
        public IPropertyType PropertyType { get; private set; }
        
        [SerializeField] private TMPro.TextMeshProUGUI costIndicator;
        [InfoBox("For manual set (view-observer role)")]
        [SerializeField] private GameplayMB gameBind;

        private Color initialColor;
        private Color dimColor;
        
        private void Awake()
        {
            initialColor = costIndicator.color;
            dimColor = initialColor * 0.5f;
        }

        private void Start()
        {
            if (gameBind.State
                    .IntProperties[PropertyType].Instance is not IUpgradableInt incrementalCost) return;
            costIndicator.text = incrementalCost.NextStepCost.ToString();
            incrementalCost.NewStep +=
                step =>
                {
                    costIndicator.text = incrementalCost.NextStepCost.ToString();
                };

            gameBind.State.Resources[incrementalCost.CostResourceType]
                .Amount.Subscribe(rAmount =>
                {
                    costIndicator.color = rAmount >= incrementalCost.NextStepCost ? initialColor : dimColor;
                });
        }
    }
}