using abc.App.Game.Code.Pure.UpgradableInt;

namespace abc.App.Game.Code.Pure.Interaction
{
    public static class UpgradeIntProperty
    {
        internal static bool Try(Gameplay context, IPropertyType type)
        {
            return ((Incremental.Runtime)context.IntProperties[type]).TryBuyNextStep();
        }
    }
}