using System;

namespace Toolkit.Resettables
{
    public class ResettableMember<T> : IResettable where T : struct
    {
        private readonly T defaultValue;
        private readonly Action<T> resetAction;

        public ResettableMember(T defaultValue, Action<T> resetAction)
        {
            this.defaultValue = defaultValue;
            this.resetAction = resetAction;
        }

        public void Reset()
        {
            resetAction.Invoke(defaultValue);
        }
    }
}