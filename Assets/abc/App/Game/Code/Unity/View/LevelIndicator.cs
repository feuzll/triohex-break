using abc.App.Game.Code.Unity.Core;
using R3;
using TMPro;
using UnityEngine;

namespace abc.App.Game.Code.Unity.View
{
    public class LevelIndicator : MonoBehaviour
    {
        [SerializeField] private GameplayMB gameBind;
        [SerializeField] private TextMeshProUGUI indicator;
        
        private void Start()
        {
            gameBind.State.Level
                .Subscribe(level => indicator.text = "Level: " + level);
        }
    }
}