namespace Toolkit.Resettables.Components
{
    public interface IComponentResetter<out T> : IResetter
    {
        T Component { get; }

        void Destroy();
    }
}