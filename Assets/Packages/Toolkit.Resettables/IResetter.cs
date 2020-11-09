namespace Toolkit.Resettables
{
    public interface IResetter
    {
        bool HasResettable { get; }
        
        void Reset();
    }
}