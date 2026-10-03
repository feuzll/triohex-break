using System;
using abc.App.Game.Code.Pure.Interaction;
using R3;

namespace abc.App.Game.Code.Pure
{
    public partial class Cell
    {
        private readonly Gameplay _context;
        public readonly Type Variation;

        internal readonly R3.ReactiveProperty<int> HealthProperty = 
            new R3.ReactiveProperty<int>(0);
        public ReadOnlyReactiveProperty<int> Health => HealthProperty;

        public Observable<Unit> Broke;
        
        public IConceptScope<Cell> Scope { get; private set; }
        public Cell(Gameplay context, Type type, IConceptScope<Cell> scope, bool activeOnCreation)
        {
            Scope = null;
            
            _context = context;
            Variation = type;

            Broke = Health.Select(h => h <= 0)
                .DistinctUntilChanged()
                .Where(broke => broke)
                .AsUnitObservable();
            
            
            context.RuntimePureCells.Add(this);

            Broke.Skip(1).Subscribe(_ => context.BreaksToNextLevel.Value--);

            if (activeOnCreation) HealthProperty.Value = context.Level.CurrentValue;
            
            context.Level.Skip(1).Subscribe((newLevel) =>
            {
                HealthProperty.Value = context.FieldSchemesCycle[
                        (newLevel-1)%context.FieldSchemesCycle.Count]
                    .CellsAsObstacles.Contains(this) ? newLevel : 0;
            });

            BallHitsCell.New.Subscribe((e) =>
            {
                (Gameplay ctx, Cell cell, Ball ball) c = e;
                if (c.cell == this)
                    ReceiveDamage(c.ball.Strength);
            });
            
            SliceHitsCell.New.Subscribe((e) =>
            {
                (Slice slice, Cell cell) info = e;
                if (info.cell == this)
                    ReceiveDamage(info.slice.Strength);
            });
            
            ObserveScopeClear(scope);
        }
        private void ObserveScopeClear(IConceptScope<Cell> scope)
        {
            if (scope is not { Pure: null }) 
                throw new NotSupportedException();
            
            Scope = scope;
            scope.OnPureClear
                .Subscribe(_ => _context.RuntimePureCells.Remove(this));
        }
        
        private void ReceiveDamage(int amount)
        {
            if (HealthProperty.Value <= 0) return;
            var actualDamage = amount > HealthProperty.Value ? HealthProperty.Value : amount;
            HealthProperty.Value -= actualDamage;
            _context.Resources[Resource.Type.Basic].AmountProperty.Value += actualDamage;
        }
    }
}