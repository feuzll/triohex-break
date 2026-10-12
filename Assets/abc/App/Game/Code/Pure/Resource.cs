using System;
using R3;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace abc.App.Game.Code.Pure
{
    [Serializable] 
    public partial class Resource
    {
        private readonly Gameplay _context;
        private readonly Resource.Type _type;

#if ODIN_INSPECTOR
        [ShowInInspector] [ReadOnly]
#endif
        internal ReactiveProperty<int> AmountProperty { get; set; } = new(); 
        public ReadOnlyReactiveProperty<int> Amount => AmountProperty;

        public Resource(Gameplay context, Resource.Type type, int amount = 0)
        {
            _context = context;
            _type = type;
            
            
            context.Resources.Add(type, this);

            if (amount < 0) amount = 0;
            AmountProperty.Value = amount;
            
            //handle prestige
            context.Level.Subscribe(level =>
            {
                if (level != 1) return;
                if (_type != Type.Basic) return;
                AmountProperty.Value = context.StartingResources[_type];
            });
        }
    }
}