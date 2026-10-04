using System;
using R3;

namespace abc.App.Game.Code.Pure
{
    [Serializable]
    public partial class Ball
    {
        public IConceptScope<Ball> Scope { get; private set; }
        
        internal readonly Gameplay Context;
        public readonly Type Variation;

        public int Speed => Context.IntProperties[Context.BallsSpeedTypes[Variation]].Value;
        public int Strength => Context.IntProperties[Context.BallsPowersTypes[Variation]].Value;
        
        public Ball(Gameplay context, Type type, IConceptScope<Ball> scope)
        {
            Scope = null;
            this.Context = context ?? throw 
                new ArgumentNullException(nameof(context));
            
            this.Variation = type;
            Context.Balls.Add(this);
            ObserveScopeClear(scope);
        }

        private void ObserveScopeClear(IConceptScope<Ball> scope)
        {
            if (scope is not { Pure: null }) 
                throw new NotSupportedException();
            
            Scope = scope;
            scope.OnPureClear
                    .Subscribe(_ => Context.Balls.Remove(this));
        }
    }
}