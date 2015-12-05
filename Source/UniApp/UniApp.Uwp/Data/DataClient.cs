﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UniApp.Uwp.Data
{
    /**
     * UniPocket	Windows - Mobile	48820-102afe26631050c23635beed
     * UniPocket	Windows - Desktop	48820-a8f54414993f4a9986fb8304
     */

    public static class DataClient
    {
        public static IObservable<byte[]> GetBytesAsObservable(
            string url,
            IDictionary<string, string> parameters = null,
            IProgress<Tuple<long, long>> progress = null,
            bool observeOnDispatcher = true)
        {
            var client = new HttpClient {Timeout = TimeSpan.FromSeconds(10)};
            var uri = GetUri(url, parameters);

            var get =
                from response in Observable.FromAsync(() => client.GetAsync(uri))
                from bytes in Observable.FromAsync(() => ReadResponseBytesAsync(response, progress))
                select bytes;

            return observeOnDispatcher ? get.ObserveOn(SynchronizationContext.Current) : get;
        }

        private static async Task<byte[]> ReadResponseBytesAsync(HttpResponseMessage response, IProgress<Tuple<long, long>> progress)
        {
            long contentLength = response.Content.Headers.ContentLength ?? -1L;
            var buffer = new byte[4096]; // 4KBs
            using (var stream = await response.Content.ReadAsStreamAsync())
            using (var memory = new MemoryStream())
            {
                int read;
                while ((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await memory.WriteAsync(buffer, 0, read);
                    progress.Report(Tuple.Create(memory.Length, contentLength));
                }

                return memory.ToArray();
            }
        }

        private static Uri GetUri(string url, IDictionary<string, string> parameters = null)
        {
            Uri uri = new Uri(url);

            if (null != parameters && 0 < parameters.Count)
            {
                StringBuilder sb = new StringBuilder();
                int count = parameters.Count;
                foreach (var pair in parameters)
                {
                    string format = --count == 0 ? "{0}={1}" : "{0}={1}&";
                    sb.AppendFormat(format, pair.Key, Uri.EscapeDataString(pair.Value));
                }

                UriBuilder uriBuilder = new UriBuilder(uri)
                {
                    Query = sb.ToString()
                };

                return uriBuilder.Uri;
            }

            return uri;
        }
    }
}