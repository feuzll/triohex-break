using abc.App.Game.Code.Pure;
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
    }
}