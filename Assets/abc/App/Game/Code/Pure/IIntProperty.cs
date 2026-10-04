using System;
using System.Diagnostics.CodeAnalysis;
using Sirenix.Serialization;

namespace abc.App.Game.Code.Pure
{
    public partial interface IIntProperty
    {
        public int Value { get; }
        public static event Action<Gameplay, IIntProperty, IPropertyType> NewInContext;

        public class Runtime
        {
            [OdinSerialize] public readonly IIntProperty Instance;

            public Runtime(Gameplay.Runtime gameplayRuntime, IIntProperty instance, IPropertyType type)
            {
                Instance = instance;
                NewInContext?.Invoke(gameplayRuntime.Context, instance, type);
            }
        }
    }
}