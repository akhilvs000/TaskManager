
namespace TaskManager
{
    public static class AppConstants
    {
		//Azure webservice URL
		public const string TasksServiceURL = "http://taskmanageravs.azurewebsites.net/";

		public const string HomePageTitle = "Task Manager";
        public const string HomePageEditTaskBtnTitle = "Edit";
        public const string HomePageCancelBtnTitle = "Cancel";
        public const string AlertOfflineTitle = "App is Offline!";
        public const string AlertOfflineInfo = "This action is not available in offline mode.";
        public const string AlertOkBtnTitle = "OK";
		public const string WaitingStatus = "Waiting";
		public const string InProgressStatus = "In Progress";
		public const string CompletedStatus = "Completed";
		public const string AppName = "Task Manager";
		public const string AlertEmptyFieldInfo = "Please enter the Task name!";
		public const string AlertErrorInfo = "Error! Please try again.";
	}
}
