using System;
using System.Collections.Generic;
using System.Linq;
using abc.App.Game.Code.Pure.Bosses;
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
                Context.IntProperties = new();
                
                Context.Tick = tick;
                Context.Tick.Subscribe(dt => Context.Time += dt);
                Context.Driver = this;
                if (Context.levelProperty.Value <= 0) Context.levelProperty.Value = 1;
                
                if (Context.BreaksToNextLevel.Value <= 0) Context.BreaksToNextLevel.Value = 1;
                
                Context.RuntimePureCells = new();
                
                Context.BrokeAllFieldCells = Context.BreaksToNextLevel
                    .Select(b => b <= 0)
                    .DistinctUntilChanged()
                    .Where(b => b)
                    .AsUnitObservable();

                Context.BrokeAllFieldCells.Subscribe(_ => Context.levelProperty.Value++);
                
                Context.Resources = new();
                Context.Balls = new();
                Context.FieldSchemesCycle = new();
                Context.Level.Skip(1).Subscribe(level =>
                {
                    if (level == 1)
                    {
                        Context.pendingPrestigePointsProperty.Value = 0;
                    }
                    else if (level % 10 == 1) Context.pendingPrestigePointsProperty.Value++;
                    var schemesCycleSize = Context.FieldSchemesCycle.Count;
                    Context.BreaksToNextLevel.Value =
                        Context.FieldSchemesCycle[(level-1)%schemesCycleSize].CellsAsObstacles.Count;

                    foreach (var pureCell in Context.RuntimePureCells) 
                        pureCell.IsArmoredProperty.Value = false;
                    
                    var requiredArmoredCells = Math.Max(0, level - 100) % 500;
                    var schemeCells = context.FieldSchemesCycle[
                        (level - 1) % context.FieldSchemesCycle.Count].CellsAsObstacles.ToArray();
                
                    for (var i = 0; i < requiredArmoredCells && i < schemeCells.Length - 1; i++) 
                        schemeCells[i].IsArmoredProperty.Value = true;
                });

                Context.BossRushRuntime = new Rush.Runtime(this, Context.BossRush);
                
                foreach (var (resourceType, loadAmount) in Context.StartingResources)
                    new Resource(Context, resourceType, loadAmount);
                
            }
        }
    }
}