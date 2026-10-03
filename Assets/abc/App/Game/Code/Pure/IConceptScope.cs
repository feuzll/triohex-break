using R3;

namespace abc.App.Game.Code.Pure
{
    public interface IConceptScope<out T>
    {
        public T Pure { get; }
        internal Observable<Unit> OnPureClear { get; }
    }
}