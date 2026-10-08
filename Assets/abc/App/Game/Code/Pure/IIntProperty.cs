using System;
using System.Diagnostics.CodeAnalysis;
using Sirenix.Serialization;

namespace abc.App.Game.Code.Pure
{
    public partial interface IIntProperty
    {
        public int Value { get; }
        //public static event Action<Gameplay, Runtime> NewInContext;

        public class Runtime
        {
            [OdinSerialize] public readonly IIntProperty Instance;
            public readonly IPropertyType instanceType;

            public Runtime(Gameplay.Runtime gameplayRuntime, IIntProperty instance, IPropertyType type)
            {
                if (gameplayRuntime is null) throw new ArgumentNullException(nameof(gameplayRuntime));
                Instance = instance;
                instanceType = type;
                gameplayRuntime.Context.IntProperties[instanceType] = this;
                //NewInContext?.Invoke(gameplayRuntime.Context, this);
            }
        }
    }
}