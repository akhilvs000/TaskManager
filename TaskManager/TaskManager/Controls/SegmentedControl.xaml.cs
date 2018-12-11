using System.Windows.Input;
using TaskManager.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TaskManager.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SegmentedControl : ContentView
	{
		public SegmentedControl()
		{
			InitializeComponent();

			InitializeSegmentSelection();
		}

		#region Bindable Properties
		public static readonly BindableProperty CommandProperty =
	   BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(SegmentedControl), null, BindingMode.TwoWay);
		#endregion

		//Gets or Sets Command Value
		public ICommand Command
		{
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}

		//Update the segment selection when Binding Context Changes
		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			var item = BindingContext as TodoItem;

			if (item != null)
			{
				switch (item.Status)
				{
					case AppConstants.WaitingStatus:
						waitingStatusFrame.BackgroundColor = Color.LightBlue;
						inProgressStatusFrame.BackgroundColor = Color.White;
						completedStatusFrame.BackgroundColor = Color.White;
						break;
					case AppConstants.InProgressStatus:
						waitingStatusFrame.BackgroundColor = Color.White;
						inProgressStatusFrame.BackgroundColor = Color.LightBlue;
						completedStatusFrame.BackgroundColor = Color.White;
						break;
					case AppConstants.CompletedStatus:
						waitingStatusFrame.BackgroundColor = Color.White;
						inProgressStatusFrame.BackgroundColor = Color.White;
						completedStatusFrame.BackgroundColor = Color.LightBlue;
						break;
				}
			}
		}

		//Initialize each segment selection
		private void InitializeSegmentSelection()
		{

			waitingStatusTapGesture.Tapped += (s, e) =>
			{
				if (Command != null && Command.CanExecute(null))
				{
					waitingStatusFrame.BackgroundColor = Color.LightBlue;
					inProgressStatusFrame.BackgroundColor = Color.White;
					completedStatusFrame.BackgroundColor = Color.White;
					Command.Execute(AppConstants.WaitingStatus);
				}
			};

			inProgressStatusTapGesture.Tapped += (s, e) =>
			{
				if (Command != null && Command.CanExecute(null))
				{
					waitingStatusFrame.BackgroundColor = Color.White;
					inProgressStatusFrame.BackgroundColor = Color.LightBlue;
					completedStatusFrame.BackgroundColor = Color.White;
					Command.Execute(AppConstants.InProgressStatus);
				}
			};

			completedStatusTapGesture.Tapped += (s, e) =>
			{
				if (Command != null && Command.CanExecute(null))
				{
					waitingStatusFrame.BackgroundColor = Color.White;
					inProgressStatusFrame.BackgroundColor = Color.White;
					completedStatusFrame.BackgroundColor = Color.LightBlue;
					Command.Execute(AppConstants.CompletedStatus);
				}
			};
		}
	}
}