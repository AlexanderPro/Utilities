using System;

namespace Utilities.Common
{
    [Serializable]
    public class EventArgs<T> : EventArgs
    {
        public T Entity { get; private set; }

        public EventArgs(T entity)
        {
            Entity = entity;
        }
    }
}
