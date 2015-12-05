using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniPocket.Shared.Observers
{
    public class HttpContentObserver : PocketObserverBase<byte[]>
    {
        public override void OnNext(byte[] value)
        {
            base.OnNext(value);

            string html = Encoding.UTF8.GetString(value, 0, value.Length);
            Debug.WriteLine(html);
        }
    }
}
