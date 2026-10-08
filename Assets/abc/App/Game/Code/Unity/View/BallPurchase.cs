using System;
using abc.App.Game.Code.Pure;
using abc.App.Game.Code.Pure.UpgradableInt;
using abc.App.Game.Code.Unity.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace abc.App.Game.Code.Unity.View
{
    [RequireComponent(typeof(Button))]
    public class BallPurchase : MonoBehaviour, IBallPurchase
    {
        private Button _buyButton;
        
        [ShowInInspector] [ReadOnly]
        public Button BuyButton => 
            _buyButton ??= GetComponent<Button>();

        [field:SerializeField]
        public Ball.Type BallType { get; private set; }
        
        [SerializeField] private TMPro.TextMeshProUGUI costIndicator;
        [InfoBox("For manual set (view-observer role)")]
        [SerializeField] private GameplayMB gameBind;
        
        private void Start()
        {
            var incrementalCost = gameBind.State
                .IntProperties[gameBind.State.BallCostsTypes[BallType]].Instance as Incremental;
            costIndicator.text = incrementalCost.NextStepCost.ToString();
            incrementalCost.NewStep +=
                step =>
                {
                    costIndicator.text = incrementalCost.NextStepCost.ToString();
                };
        }
    }
}