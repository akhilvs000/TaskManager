using DryIoc;
using Prism;
using Prism.DryIoc;
using Prism.Ioc;
using TaskManager.DataAccessLayer;
using TaskManager.Interfaces;
using TaskManager.Model;
using TaskManager.Services;
using TaskManager.ViewModels;
using TaskManager.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TaskManager.Helpers;
[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace TaskManager
{
    public partial class App
	{   
        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public App() : this(null) { }
        
        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            await NavigationService.NavigateAsync("NavigationPage/TaskHomePage");
        }

		protected override void RegisterTypes(IContainerRegistry containerRegistry)
		{
			containerRegistry.RegisterForNavigation<NavigationPage>();
			containerRegistry.RegisterForNavigation<TaskHomePage, TaskHomePageViewModel>(); 

            IocContainer.Instance.Container.Register<IRepository<TodoDBItem>, BaseRepository<TodoDBItem>>();
            IocContainer.Instance.Container.Register<ITasksService, TasksService>();
            IocContainer.Instance.Container.Register<IPlatformService, PlatformService>();

        }

    }
}
