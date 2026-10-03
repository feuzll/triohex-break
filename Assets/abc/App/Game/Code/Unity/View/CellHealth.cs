using abc.App.Game.Code.Unity.Core;
using R3;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;

namespace abc.App.Game.Code.Unity.View
{
    [RequireComponent(typeof(TextMeshPro))]
    [RequireComponent(typeof(PositionConstraint))]
    public class CellHealth : MonoBehaviour
    {
        [SerializeField] private CellMB cellBind;

        private TextMeshPro _indicator;

        private void Awake()
        {
            _indicator = GetComponent<TextMeshPro>();
        }

        private void Start()
        {
            cellBind.Pure.Health.Subscribe(next =>
            {
                _indicator.text = cellBind.Pure.Health.CurrentValue.ToString();
                gameObject.SetActive(cellBind.Pure.Health.CurrentValue > 0);
            });
        }

        [Button]
        private void MapToHierarchyIndex(Transform cellsHierarchy)
        {
            cellBind = null;
            var index = transform.GetSiblingIndex();
            if (cellsHierarchy.childCount >= index 
                && cellsHierarchy.GetChild(index)
                    .TryGetComponent<CellMB>(out var cell))
            {
                cellBind = cell;
            }
            else 
                Debug.LogWarning
                ($"{cellsHierarchy.GetChild(index).name} " + 
                 $"has no {nameof(CellMB)}",
                    cellsHierarchy.GetChild(index));
        }

        [Button]
        private void MapToConstraintSource()
        {
            cellBind = GetComponent<PositionConstraint>()
                .GetSource(0).sourceTransform
                .GetComponent<CellMB>();
        }
    }
}