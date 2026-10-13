using System;

namespace abc.App.Game.Code.Pure.UpgradableInt
{
    public interface IUpgradableInt : IIntProperty
    {
        public int NextStepCost { get; }
        public int Step { get; }
        public bool IsMaxed { get; }
        public Resource.Type CostResourceType { get; }
        public event Action<int> NewStep;
        internal void AdvanceStep();
    }
}