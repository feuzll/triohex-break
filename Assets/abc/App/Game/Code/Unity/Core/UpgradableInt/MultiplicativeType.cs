using abc.App.Game.Code.Pure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace abc.App.Game.Code.Unity.Core.UpgradableInt
{
    [CreateAssetMenu(fileName = "MultiplicativeType", menuName = "abc/App/Game/UpgradableInt/MultiplicativeType", order = 0)]
    public class MultiplicativeType : SerializedScriptableObject, IPropertyType
    {
        [OdinSerialize] internal Pure.UpgradableInt.Multiplicative runtimeTemplate;
    }
}