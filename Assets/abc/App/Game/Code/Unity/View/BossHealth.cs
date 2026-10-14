using abc.App.Game.Code.Unity.Core;
using R3;
using TMPro;
using UnityEngine;

namespace abc.App.Game.Code.Unity.View
{
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
            bossBind.Instance.Health.Subscribe(next =>
            {
                _indicator.text = bossBind.Instance.Health.CurrentValue.ToString();
                gameObject.SetActive(bossBind.Instance.Health.CurrentValue > 0);
            });
        }

    }
}