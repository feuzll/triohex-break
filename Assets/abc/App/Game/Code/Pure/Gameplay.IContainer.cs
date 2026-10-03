namespace abc.App.Game.Code.Pure
{
    public partial class Gameplay
    {
        public interface IContainer
        {
            public Gameplay Instance { get; }
            private void Validate() => Instance.Validate();
        }
        
    }
}