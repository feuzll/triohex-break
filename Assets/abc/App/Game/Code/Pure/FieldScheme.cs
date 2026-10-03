using System;
using System.Collections.Generic;
using R3;

namespace abc.App.Game.Code.Pure
{
    public class FieldScheme
    {
        private readonly Gameplay _gameplay;

        internal FieldScheme(Gameplay gameplay,
            HashSet<Cell> cellsAsObstacles,
            IConceptScope<FieldScheme> scope)
        {
            _gameplay = gameplay;
            CellsAsObstacles = cellsAsObstacles;
            Scope = scope;
            gameplay.FieldSchemesCycle.Add(this);
            ObserveScopeClear(scope);
        }
        
        private void ObserveScopeClear(IConceptScope<FieldScheme> scope)
        {
            if (scope is not { Pure: null }) 
                throw new NotSupportedException();
            
            Scope = scope;
            scope.OnPureClear
                .Subscribe(_ =>
                    _gameplay.FieldSchemesCycle.Remove(this));
        }
        
        internal HashSet<Cell> CellsAsObstacles { get; }
        public IConceptScope<FieldScheme> Scope { get; private set; }
    }
}