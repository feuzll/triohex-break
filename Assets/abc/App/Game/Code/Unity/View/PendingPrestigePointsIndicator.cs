using System;
using abc.App.Game.Code.Unity.Core;
using R3;
using UnityEngine;

namespace abc.App.Game.Code.Unity.View
{
    public class PendingPrestigePointsIndicator : MonoBehaviour
    {
        [SerializeField] private GameplayMB gameBind;
        [SerializeField] private TMPro.TextMeshProUGUI indicator;
        
        private void Start()
        {
            gameBind.State.PendingPrestigePoints
                .Subscribe(points => indicator.text = "+" + points);
        }
    }
}