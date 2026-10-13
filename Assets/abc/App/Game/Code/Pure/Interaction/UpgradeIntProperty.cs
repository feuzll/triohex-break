using abc.App.Game.Code.Pure.UpgradableInt;

namespace abc.App.Game.Code.Pure.Interaction
{
    public static class UpgradeIntProperty
    {
        internal static bool Try(Gameplay context, IPropertyType type)
        {
            var upgradableInt = (IUpgradableInt)context.IntProperties[type].Instance;
            if (upgradableInt.IsMaxed) return false;
            if (!context.Resources[upgradableInt.CostResourceType].TrySpend(upgradableInt.NextStepCost)) return false;
            upgradableInt.AdvanceStep();
            return true;
        }
    }
}