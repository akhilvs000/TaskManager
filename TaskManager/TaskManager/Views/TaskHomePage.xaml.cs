using System;
using TaskManager.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TaskManager.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TaskHomePage : ContentPage
	{
		private TaskHomePageViewModel _vm;
		private TaskHomePageViewModel viewModel => _vm ?? (_vm = BindingContext as TaskHomePageViewModel);

		public TaskHomePage()
		{

			InitializeComponent();
			taskListView.ItemSelected += (sender, e) =>
			{
				((ListView)sender).SelectedItem = null;
			};
		}

        protected override async void OnAppearing()
		{
			base.OnAppearing();

			await viewModel.GetTaskListAsync();
		}
	}
}