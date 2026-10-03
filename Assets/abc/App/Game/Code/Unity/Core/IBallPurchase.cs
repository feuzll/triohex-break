using UnityEngine.UI;

namespace abc.App.Game.Code.Unity.Core
{
    public interface IBallPurchase
    {
        public Button BuyButton { get; }
        public Pure.Ball.Type BallType { get; }
    }
}