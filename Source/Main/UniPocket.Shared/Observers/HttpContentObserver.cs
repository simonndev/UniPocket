using System;

namespace UniPocket.Shared.Observers
{
    public abstract class HttpContentObserverBase<T> : IObserver<T>
    {
        public event Action<T> ParseContentCompleted;
        public event Action<Exception> ErrorCallback; 
         
        public T HtmlContent { get; protected set; }

        public virtual void OnCompleted()
        {
            ParseContentCompleted?.Invoke(this.HtmlContent);
        }

        public virtual void OnError(Exception error)
        {
            ErrorCallback?.Invoke(error);
        }

        public virtual void OnNext(T value)
        {
            this.HtmlContent = value;
        }
    }
}