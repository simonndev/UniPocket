using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniPocket.Shared.Observers
{
    public class PocketObserverBase<TResult> : IObserver<TResult>
    {
        public PocketObserverBase()
        {
            this.Result = default(TResult);
        }

        public event Action<TResult> Completed;
        public event Action<Exception> Error;

        public TResult Result { get; protected set; }

        public virtual void OnCompleted()
        {
            Completed?.Invoke(Result);
        }

        public virtual void OnError(Exception error)
        {
            Error?.Invoke(error);
        }

        public virtual void OnNext(TResult value)
        {
            this.Result = value;
        }
    }
}
