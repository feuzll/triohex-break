using System.Collections.Generic;
using System.Linq;
using abc.App.Game.Code.Pure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace abc.App.Game.Code.Unity.Core
{
    public class FieldSchemeMB : MonoBind<Pure.FieldScheme>
    {
        protected override void OnNewPureState()
        {
            //throw new System.NotImplementedException();
        }

        protected override void ClearBinds()
        {
            //throw new System.NotImplementedException();
        }

        [DisableInEditorMode]
        internal GameplayMB ContextBind
        {
            get => contextBindField;
            set //runtime manual inspector change
            {
                if (Pure != null) ClearBinds();
                contextBindField = value;
                if (value == null)
                    Pure = null;
                else
                {
                    var newPureScheme = 
                        Context.RuntimePureCells.Where(c =>
                            CellsAsObstacles.Contains(c.Scope)).ToHashSet();
                    Pure = new Pure.FieldScheme(value.State, newPureScheme, this);
                }
            }
        }
        [SerializeField] [ReadOnly] private GameplayMB contextBindField;
        private Pure.Gameplay Context =>  ContextBind?.State;
        
        [OdinSerialize] public HashSet<CellMB> CellsAsObstacles { get; protected set; }
        
        [Button]
        protected void GenerateFromChildCollidersOverlap()
        {
            CellsAsObstacles = new HashSet<CellMB>();
            var schemeColliders = GetComponentsInChildren<Collider2D>();
            var fieldCells =
                FindObjectsByType<CellMB>(
                    FindObjectsInactive.Include, FindObjectsSortMode.None);

            //Debug.Log(fieldCells.Length);
            
            foreach (var cell in fieldCells)
            {
                cell.gameObject.SetActive(true);
                var cellCollider = cell.gameObject.GetComponent<Collider2D>();
                foreach (var schemeCollider in schemeColliders)
                    if (schemeCollider.Distance(cellCollider).isOverlapped)
                        CellsAsObstacles.Add(cell);
            }
            Highlight();
        }

        [Button]
        protected void Highlight()
        {
            var fieldCells = FindObjectsByType<CellMB>(
                FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var cell in fieldCells)
                cell.gameObject.SetActive(CellsAsObstacles.Contains(cell));
        }

        [Button]
        protected void ShowAllCells()
        {
            var fieldCells = FindObjectsByType<CellMB>(
                FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var cell in fieldCells)
                cell.gameObject.SetActive(true);
        }
    }
}