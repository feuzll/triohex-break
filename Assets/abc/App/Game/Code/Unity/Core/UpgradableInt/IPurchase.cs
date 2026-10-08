using abc.App.Game.Code.Pure;
using Unity.Properties;
using UnityEngine.UI;

namespace abc.App.Game.Code.Unity.Core.UpgradableInt
{
    public interface IPurchase
    {
        public Button BuyButton { get; }
        public IPropertyType PropertyType { get; }
    }
}