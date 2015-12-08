
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Mvvm;
using Prism.Unity.Windows;
using Prism.Windows.AppModel;
using Prism.Windows.Navigation;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.UI.Notifications;
using Windows.UI.WebUI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using UniPocket.Shared.Services;

namespace UniPocket
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : PrismUnityApplication
    {
        // Bootstrap: App singleton service declarations
        private TileUpdater _tileUpdater;

        public IEventAggregator EventAggregator { get; set; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            this.InitializeComponent();
        }

        protected override Task OnInitializeAsync(IActivatedEventArgs args)
        {
            EventAggregator = new EventAggregator();

            // Register MvvmAppBase services with the container so that view models can take dependencies on them
            Container.RegisterInstance<ISessionStateService>(SessionStateService);
            Container.RegisterInstance<INavigationService>(NavigationService);
            Container.RegisterInstance<IEventAggregator>(EventAggregator);
            //Container.RegisterInstance<IResourceLoader>(new ResourceLoaderAdapter(new ResourceLoader()));

            // Register any app specific types with the container
            Container.RegisterType<IPocketService, PocketService>(new ContainerControlledLifetimeManager());
            //Container.RegisterType<ILocalStorageService, JsonDataSourceService>(new ContainerControlledLifetimeManager());

            // Set a factory for the ViewModelLocator to use the container to construct view models so their
            // dependencies get injected by the container
            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
            {
                var viewModelTypeName = string.Format("UniPocket.Shared.ViewModels.{0}ViewModel, UniPocket.Shared", viewType.Name);
                var viewModelType = Type.GetType(viewModelTypeName);
                if (viewModelType == null)
                {
                    viewModelTypeName = string.Format(CultureInfo.InvariantCulture, "UniPocket.Shared.ViewModels.{0}ViewModel, UniPocket.Shared.Windows, Version=1.0.0.0, Culture=neutral", viewType.Name);
                    viewModelType = Type.GetType(viewModelTypeName);
                }

                return viewModelType;
            });

            _tileUpdater = TileUpdateManager.CreateTileUpdaterForApplication();
            //_tileUpdater.StartPeriodicUpdate(new Uri(Constants.ServerAddress + "/api/TileNotification"), PeriodicUpdateRecurrence.HalfHour);

            return base.OnInitializeAsync(args);
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
        {
            if (!string.IsNullOrEmpty(args?.Arguments))
            {
                // The app was launched from a Secondary Tile
                // TODO: swich for the args
                // Navigate to the item's page
                NavigationService.Navigate("ComicViewer", args.Arguments);
            }
            else
            {
                // Navigate to the sign-in page
                NavigationService.Navigate("SignIn", null);
            }

            Window.Current.Activate();
            return Task.FromResult<object>(null);
        }

        protected override void OnRegisterKnownTypesForSerialization()
        {
            //SessionStateService.RegisterKnownType(typeof(ComicIcons));
            //SessionStateService.RegisterKnownType(typeof(Feature));
            //SessionStateService.RegisterKnownType(typeof(Featured));
            //SessionStateService.RegisterKnownType(typeof(Features));
            //SessionStateService.RegisterKnownType(typeof(FeatureItem));
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
