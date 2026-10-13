using System;

namespace abc.App.Game.Code.Pure.UpgradableInt
{
    public interface IUpgradableInt : IIntProperty
    {
        public int NextStepCost { get; }
        public int Step { get; }
        public Resource.Type CostResourceType { get; }
        public event Action<int> NewStep;
    }
}