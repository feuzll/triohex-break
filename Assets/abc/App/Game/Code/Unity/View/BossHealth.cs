using abc.App.Game.Code.Unity.Core;
using R3;
using TMPro;
using UnityEngine;

namespace abc.App.Game.Code.Unity.View
{
    [RequireComponent(typeof(TMPro.TextMeshPro))]
    public class BossHealth : MonoBehaviour
    {
        [SerializeField] private BossMB bossBind;

        private TextMeshPro _indicator;

        private void Awake()
        {
            _indicator = GetComponent<TextMeshPro>();
        }

        private void Start()
        {
            bossBind.Pure.Health.Subscribe(next =>
            {
                _indicator.text = bossBind.Pure.Health.CurrentValue.ToString();
                gameObject.SetActive(bossBind.Pure.Health.CurrentValue > 0);
            });
        }

    }
}