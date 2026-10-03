using System;
using R3;

namespace abc.App.Game.Code.Pure
{
    public partial class Slice
    {
        public partial class Mana
        {
            public class Runtime : Slice.IBudget
            {
                private readonly Mana _state;

                public Runtime(Gameplay context)
                {
                    _state = context.Mana;
                    context.ManaRuntime = this;
                    
                    context.Tick.Subscribe(timeDelta =>
                    {
                        var newAmount = _state.Amount.CurrentValue +
                            _state.Limit.CurrentValue * _state.ManaRegenPercentPerSecond * timeDelta;
                        _state.AmountProperty.Value =
                            newAmount <= _state.LimitProperty.Value ? newAmount 
                                : _state.LimitProperty.Value;
                    });
                }

                public bool CanStart => _state.Amount.CurrentValue > 0;
                public float Request(float worldLength)
                {
                    float affordable = _state.Amount.CurrentValue / _state.CostPerUnit;
                    float granted = Math.Min(worldLength, affordable);
                    _state.AmountProperty.Value -= (granted * _state.CostPerUnit);
                    return granted;
                }
            }
        }
    }
}