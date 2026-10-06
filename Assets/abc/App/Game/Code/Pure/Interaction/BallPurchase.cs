using System;
using abc.App.Game.Code.Pure.UpgradableInt;

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
            if (((Incremental.Runtime)context.BallCostsRuntimes[type]).TryBuyNextStep() == false)
            {
                ball = null;
                return false;
            }
            
            ball = new Ball(context, type, provideScopeCallback());
            return true;
        }
    }
}