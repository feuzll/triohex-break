using abc.App.Game.Code.Pure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace abc.App.Game.Code.Unity.Core.UpgradableInt
{
    [CreateAssetMenu(fileName = "IncrementalType", menuName = "abc/App/Game/UpgradableInt/IncrementalType", order = 0)]
    public class IncrementalType : SerializedScriptableObject, IPropertyType
    {
        [OdinSerialize] internal Pure.UpgradableInt.Incremental runtimeTemplate;
    }
}