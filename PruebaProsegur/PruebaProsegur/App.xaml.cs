using Prism;
using Prism.Ioc;
using PruebaProsegur.ViewModels;
using PruebaProsegur.Views;
using Xamarin.Essentials.Interfaces;
using Xamarin.Essentials.Implementation;
using Xamarin.Forms;
using PruebaProsegur.Services;

namespace PruebaProsegur
{
    public partial class App
    {
        public App(IPlatformInitializer initializer)
            : base(initializer)
        {
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            await NavigationService.NavigateAsync("NavigationPage/LoginPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();
            containerRegistry.Register<IApiService, ApiService>();
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>();
            containerRegistry.RegisterForNavigation<ExplicationPage, ExplicationPageViewModel>();
        }
    }
}
