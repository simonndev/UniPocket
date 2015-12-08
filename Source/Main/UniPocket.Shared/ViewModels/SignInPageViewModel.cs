using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using UniPocket.Shared.Observers;
using UniPocket.Shared.Services;

namespace UniPocket.Shared.ViewModels
{
    public class SignInPageViewModel : ViewModelBase
    {
        private readonly IPocketService _service;
        private string _username;
        private string _password;
        private bool _isRememberMe;

        public SignInPageViewModel(IPocketService pocketService)
        {
            this._service = pocketService;
        }

        public string Username
        {
            get { return _username; }
            set { base.SetProperty(ref _username, value); }
        }

        public string Password
        {
            get { return _password; }
            set { base.SetProperty(ref _password, value); }
        }

        public bool IsRememberMe
        {
            get { return _isRememberMe; }
            set { base.SetProperty(ref _isRememberMe, value); }
        }

        public override async void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            base.OnNavigatedTo(e, viewModelState);

            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"consumer_key", "48820-102afe26631050c23635beed"},
                {"redirect_uri", "pocketapp48820:authorizationFinished"}
            };

            PocketObserverBase<string> observer = new PocketObserverBase<string>();
            observer.Error += (exception) =>
            {
                Debug.WriteLine(exception.Message);
            };
            observer.Completed += (content) =>
            {
                Debug.WriteLine(content);
            };

            this._service.SubscribeOAuthRequest(observer, parameters);
        }
    }
}
