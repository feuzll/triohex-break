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
            
            if (UpgradeIntProperty.Try(context, context.BallCostsTypes[type]) == false)
            {
                ball = null;
                return false;
            }
            
            ball = new Ball(context, type, provideScopeCallback());
            return true;
        }
    }
}