using abc.App.Game.Code.Pure;
using UnityEngine;

namespace abc.App.Game.Code.Unity.Core
{
    [CreateAssetMenu(fileName = "ConstantInt", menuName = "abc/App/Game/ConstantInt", order = 0)]
    public class ConstantInt : ScriptableObject, IIntProperty, IPropertyType
    {
        [field: SerializeField] public int Value { get; private set; }
    }
}