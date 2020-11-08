namespace Toolkit.Resettables
{
    public interface IResetter
    {
        void Reset();
    }

    public interface IResetter<out T> : IResetter
    {
    }
}