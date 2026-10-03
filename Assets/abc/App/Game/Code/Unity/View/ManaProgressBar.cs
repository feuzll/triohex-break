using System;
using abc.App.Game.Code.Unity.Core;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace abc.App.Game.Code.Unity.View
{
    [RequireComponent(typeof(Image))]
    public class ManaProgressBar : MonoBehaviour
    {
        [SerializeField] private GameplayMB gameBind;
        private Image indicator;

        private void Awake()
        {
            indicator = GetComponent<Image>();
        }

        private void Start()
        {
            gameBind.State.Mana.Amount.Subscribe(current =>
            {
                var max = gameBind.State.Mana.Limit.CurrentValue;
                indicator.fillAmount = current / max;
            });
        }
    }
}