using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using UniApp.Uwp.Data;
using UniPocket.Shared.Observers;
using UniPocket.Shared.Services;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UniPocket
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            //Start();
        }

        private void Start()
        {
            string url = "http://www.gocomics.com/api/features.json?client_code=FHE576CHD922";

            var progressIndicator = new Progress<Tuple<long, long>>();
            progressIndicator.ProgressChanged += (s, value) =>
            {
                Debug.WriteLine("% {0}/{1}", value.Item1, value.Item2);
            };

            //			await FeatureModelCollection.SearchTermAsync(string.Empty, progressIndicator);

            //PocketService service = new PocketService();
            //service.s(new HttpContentObserver(), progressIndicator);
        }
    }
}
