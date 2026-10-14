using System;
using abc.App.Game.Code.Pure;
using abc.App.Game.Code.Unity.Core;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace abc.App.Game.Code.Unity.View
{
    public class BossRushControls : MonoBehaviour
    {
        [SerializeField] private Button enter;
        [SerializeField] private Button exit;
        [SerializeField] private GameplayMB gameBind;

        private void Start()
        {
            gameBind.State.BossRush.IsFighting.Subscribe(inFight =>
            {
                var canAffordBossFight = gameBind.State.Resources[Resource.Type.SS]
                    .Amount.CurrentValue >= gameBind.State.SSCostPerBossRushLevel * gameBind.State.BossRush.Level.CurrentValue; 
                
                enter.gameObject.SetActive(!inFight);
                enter.interactable = canAffordBossFight;
                exit.gameObject.SetActive(inFight);
            });

            gameBind.State.Resources[Resource.Type.SS].Amount.Subscribe(newAmount =>
                enter.interactable = newAmount >= (gameBind.State
                    .SSCostPerBossRushLevel * gameBind.State.BossRush.Level.CurrentValue));
        }
    }
}