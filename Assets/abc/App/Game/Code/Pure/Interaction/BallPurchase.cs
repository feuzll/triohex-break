using System;

namespace abc.App.Game.Code.Pure.Interaction
{
    internal static class BallPurchase
    {
        internal static bool Try(Gameplay context, Ball.Type type, 
            Func<IConceptScope<Ball>> provideScopeCallback, out Ball ball)
        {
            if (provideScopeCallback == null)
                throw new ArgumentNullException(nameof(provideScopeCallback));
            
            var basicResource = context.Resources[Resource.Type.Basic];
            if (basicResource.Amount.CurrentValue < context.BallCosts[type])
            {
                ball = null;
                return false;
            }

            basicResource.AmountProperty.Value -= context.BallCosts[type];
            ball = new Ball(context, type, provideScopeCallback());
            return true;
        }
    }
}