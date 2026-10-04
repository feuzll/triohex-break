using System;
using System.Collections.Generic;
using R3;

namespace abc.App.Game.Code.Pure
{
    public partial class Gameplay
    {
        public partial class Runtime : IRules
        {
            internal Gameplay Context;
            
            public Runtime(Gameplay context, Observable<float> tick)
            {
                Context = context;
                context.IntProperties = new();
                
                context.Tick = tick;
                context.Tick.Subscribe(dt => context.Time += dt);
                context.Driver = this;
                if (context.levelProperty.Value <= 0) context.levelProperty.Value = 1;
                
                if (context.BreaksToNextLevel.Value <= 0) context.BreaksToNextLevel.Value = 1;
                
                context.RuntimePureCells = new();
                
                context.BrokeAllFieldCells = context.BreaksToNextLevel
                    .Select(b => b <= 0)
                    .DistinctUntilChanged()
                    .Where(b => b)
                    .AsUnitObservable();

                context.BrokeAllFieldCells.Subscribe(_ => context.levelProperty.Value++);
                
                context.Resources = new();
                context.Balls = new();
                context.FieldSchemesCycle = new();
                context.Level.Skip(1).Subscribe(level =>
                {
                    var schemesCycleSize = context.FieldSchemesCycle.Count;
                    context.BreaksToNextLevel.Value =
                        context.FieldSchemesCycle[(level-1)%schemesCycleSize].CellsAsObstacles.Count;
                });

                IIntProperty.NewInContext += (gameplay, property, type) =>
                {
                    if (gameplay != Context) return;
                    Context.IntProperties[type] = property;
                };
                
                foreach (var (resourceType, amount) in context.StartingResources)
                    new Resource(context, resourceType, amount);
                
            }
        }
    }
}