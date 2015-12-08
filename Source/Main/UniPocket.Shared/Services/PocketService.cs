using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.Security.Authentication.Web.Core;
using Windows.Security.Credentials;
using UniApp.Uwp.Data;

namespace UniPocket.Shared.Services
{
    public interface IPocketService
    {
        void SubscribeBytes(string url, IObserver<byte[]> observer, IDictionary<string, string> parameters = null, IProgress<Tuple<long, long>> progress = null);
        void SubscribeOAuthRequest(IObserver<string> observer, IDictionary<string, string> parameters = null, IProgress<Tuple<long, long>> progress = null);
    }

    /**
     * UniPocket	Windows - Mobile	48820-102afe26631050c23635beed
     * UniPocket	Windows - Desktop	48820-a8f54414993f4a9986fb8304
     */

    public class PocketService : IPocketService
    {
        private const int TIMEOUT = 10; // TODO: should be read from configuration file.
        private static readonly object SyncRoot = new object();
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(TIMEOUT);

        // https://getpocket.com/developer/docs/getstarted/windows8
        private static string Authentication = "https://getpocket.com/v3/oauth/request";
        private static string Authorization = "https://getpocket.com/v3/oauth/authorize";
        private static string Add = "https://getpocket.com/v3/add";
        private static string Modify = "https://getpocket.com/v3/send";
        private static string Retrieve = "https://getpocket.com/v3/get";

        private static readonly IDictionary<Guid, IDisposable> CurrentSubscriptions =
            new Dictionary<Guid, IDisposable>();

        public void SubscribeBytes(string url, IObserver<byte[]> observer, IDictionary<string, string> parameters = null, IProgress<Tuple<long, long>> progress = null)
        {
            var subscriptionKey = Guid.NewGuid();

            var observable = DataClient.GetBytesAsObservable(url, parameters, progress);
            var disposal = observable.Finally(() => CleanupSubscription(subscriptionKey)).Subscribe(observer);

            lock (SyncRoot)
            {
                CurrentSubscriptions.Add(subscriptionKey, disposal);
            }
        }

        public void SubscribeOAuthRequest(IObserver<string> observer, IDictionary<string, string> parameters = null, IProgress<Tuple<long, long>> progress = null)
        {
            var subscriptionKey = Guid.NewGuid();

            var observable = DataClient.PostAsObservable(Authentication, parameters);
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
                    Debug.WriteLine(
                        string.Format("GoComicsService::Cleanup - CurrentSubscriptions does not contain key {0}", key));
                    return;
                }

                Debug.WriteLine(string.Format("PocketService::Cleanup - Disposing subscription {0}", key));
                CurrentSubscriptions[key].Dispose();
                CurrentSubscriptions.Remove(key);
            }
        }
    }
}
