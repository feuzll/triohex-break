using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace abc.App.Game.Code.Unity.Core
{
    [CreateAssetMenu(fileName = "IncrementalUpgradeType", menuName = "abc/App/Game/IncrementalUpgradeType", order = 0)]
    public class IncrementalUpgradeType : SerializedScriptableObject, Pure.Upgrades.Incremental.IType
    {
        [OdinSerialize] internal Pure.Upgrades.Incremental runtimeTemplate;
    }
}