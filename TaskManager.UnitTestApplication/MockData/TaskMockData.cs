using System.Collections.Generic;
using TaskManager.Model;

namespace TaskManager.UnitTest.MockData
{
    public static class TaskMockData
    {
		public static List<TodoItem> ReturnServiceTaskList()
		{
			var _todoItems = new List<TodoItem>()
			{
				new TodoItem()
				{
					 Id = "1",
					Name = "Azure Task",
					Status = AppConstants.InProgressStatus
				},
				new TodoItem()
				{
					 Id = "2",
					Name = "Xamarin Task",
					Status = AppConstants.WaitingStatus
				},
				new TodoItem()
				{
					 Id = "3",
					Name = "POC",
					Status = AppConstants.CompletedStatus
				}
			};
			return _todoItems;
		}

		public static List<TodoDBItem> ReturnDBTaskList()
		{
			var _todoDBItems = new List<TodoDBItem>()
			{
				new TodoDBItem()
				{
					 Id = "1",
					Name = "Azure Task",
					Status = AppConstants.InProgressStatus
				},
				new TodoDBItem()
				{
					 Id = "2",
					Name = "Xamarin Task",
					Status = AppConstants.WaitingStatus
				},
				new TodoDBItem()
				{
					 Id = "3",
					Name = "POC",
					Status = AppConstants.CompletedStatus
				}
			};
			return _todoDBItems;
		}
	}
}
