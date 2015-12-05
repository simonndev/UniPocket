using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using UniApp.Uwp.Data;

namespace UniPocket.Shared.Services
{
    public  class PocketService
    {
        private const int TIMEOUT = 10; // TODO: should be read from configuration file.
        private static readonly object SyncRoot = new object();
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(TIMEOUT);
        private static readonly IDictionary<Guid, IDisposable> CurrentSubscriptions = new Dictionary<Guid, IDisposable>();

        public void SubscribeBytes(IObserver<byte[]> observer, IProgress<Tuple<long, long>> progress = null)
        {
            var subscriptionKey = Guid.NewGuid();

            var observable = DataClient.GetBytesAsObservable("http://www.gocomics.com/api/features.json?client_code=FHE576CHD922", null, progress);
            var disposal = observable.Finally(() => CleanupSubscription(subscriptionKey)).Subscribe(observer);

            lock (SyncRoot)
            {
                CurrentSubscriptions.Add(subscriptionKey, disposal);
            }
        }

        private static void CleanupSubscription(Guid key)
        {
            lock (SyncRoot)
            {
                if (!CurrentSubscriptions.ContainsKey(key))
                {
                    Debug.WriteLine(string.Format("GoComicsService::Cleanup - CurrentSubscriptions does not contain key {0}", key));
                    return;
                }

                Debug.WriteLine(string.Format("PocketService::Cleanup - Disposing subscription {0}", key));
                CurrentSubscriptions[key].Dispose();
                CurrentSubscriptions.Remove(key);
            }
        }
    }
}
