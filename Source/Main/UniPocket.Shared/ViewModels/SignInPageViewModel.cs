using Prism.Commands;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using HtmlAgilityPack;
using UniPocket.Shared.Observers;
using UniPocket.Shared.Services;

namespace UniPocket.Shared.ViewModels
{
    public class SignInPageViewModel : ViewModelBase
    {
        private readonly IPocketService _service;
        private string _email;
        private string _username;
        private string _password;
        private bool _isRememberMe;
        private string _requestToken;
        private string _accessToken;
        private IDictionary<string, string> _authorizationParameters;

        public SignInPageViewModel(IPocketService pocketService)
        {
            this._service = pocketService;
            this.SignInCommand = new DelegateCommand(this.DoSignIn);
        }

        #region Properties

        public string Email
        {
            get { return this._email; }
            set { base.SetProperty(ref this._email, value); }
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

        #endregion

        #region Commands

        public ICommand SignInCommand { get; private set; }

        #endregion

        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            base.OnNavigatedTo(e, viewModelState);

            // Sends an OAuth request to get token
            IDictionary<string, string> requestTokenParameters = new Dictionary<string, string>
            {
                {"consumer_key", "48820-102afe26631050c23635beed"},
                {"redirect_uri", "pocketapp48820:authorizationFinished"}
            };

            ObtainRequestTokenObserver requestTokenObserver = new ObtainRequestTokenObserver();
            requestTokenObserver.ErrorCallback += (exception) =>
            {
                Debug.WriteLine(exception.Message);
            };
            requestTokenObserver.ParseRequestTokenCompleted += (requestToken) =>
            {
                Debug.WriteLine(requestToken); // 72efebb4-fa49-8cce-097e-9da223 SUCCESS

                // TODO: Parse the token
                this._requestToken = requestToken;

                IDictionary<string, string> requestSignInParameters = new Dictionary<string, string>
                {
                    {"request_token", this._requestToken},
                    {"redirect_uri", "pocketapp48820"}, // pocketapp48820:authorizationFinished
                    {"mobile", "0"},
                    {"force", "login"},
                    {"webauthenticationbroker", "1"}
                };

                RequestSignInObserver observer = new RequestSignInObserver();
                observer.ParseSignInParametersCompleted += (authorizationParameters) =>
                {
                    this._authorizationParameters = authorizationParameters;
                };

                this._service.SubscribeRedirectUserForAuthorization(observer, requestSignInParameters);
            };

            this._service.SubscribeOAuthRequestToken(requestTokenObserver, requestTokenParameters);
        }

        private void DoSignIn()
        {
            if (!this._authorizationParameters.ContainsKey("feed_id") ||
                !this._authorizationParameters.ContainsKey("password"))
            {
                this._authorizationParameters.Add("feed_id", this.Username);
                this._authorizationParameters.Add("password", this.Password);
            }

            PocketObserverBase<string> observer = new PocketObserverBase<string>();
            observer.Error += (exception) =>
            {
                Debug.WriteLine(exception.Message);
            };
            observer.Completed += (content) =>
            {
                Debug.WriteLine(content);

                PocketObserverBase<string> redirectUserObserver = new PocketObserverBase<string>();
                observer.Error += (exception) =>
                {
                    Debug.WriteLine(exception.Message);
                };
                observer.Completed += (content1) =>
                {
                    // Authorization completed - Convert the Request Token into a Pocket Access Token
                    PocketObserverBase<string> authorizationObserver = new PocketObserverBase<string>();
                    observer.Error += (exception) =>
                    {
                        Debug.WriteLine(exception.Message);
                    };
                    observer.Completed += (authorizationContent) =>
                    {
                        Debug.WriteLine(authorizationContent);
                    };

                    IDictionary<string, string> authorizationParams = new Dictionary<string, string>
                    {
                        {"code", this._requestToken},
                        {"consumer_key", "48820-102afe26631050c23635beed::authorizationFinished"},
                    };

                    this._service.SubscribeAuthorization(authorizationObserver, authorizationParams);
                };

                IDictionary<string, string> redirectUserParams = new Dictionary<string, string>
                {
                    {"request_token", this._requestToken},
                    {"consumer_key", "48820-102afe26631050c23635beed::authorizationFinished"},
                };

                this._service.SubscribeRedirectUserForAuthorization(redirectUserObserver, redirectUserParams);
            };

            this._service.SubscribeSignInProcess(observer, this._authorizationParameters);
        }
    }
}