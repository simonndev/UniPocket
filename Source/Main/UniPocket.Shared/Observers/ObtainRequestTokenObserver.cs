using System;

namespace UniPocket.Shared.Observers
{
    public class ObtainRequestTokenObserver : HttpContentObserverBase<string>
    {
        public event Action<string> ParseRequestTokenCompleted;

        public override void OnCompleted()
        {
            string requestToken = this.HtmlContent.Split(new[] { '=' })[1];

            ParseRequestTokenCompleted?.Invoke(requestToken);
        }
    }
}