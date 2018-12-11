using DryIoc;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Helpers;
using TaskManager.Interfaces;
using TaskManager.Model;
using Xamarin.Forms;

namespace TaskManager.ViewModels
{
    public class TaskHomePageViewModel : ViewModelBase
    {
        #region Readonly Properties
        readonly IRepository<TodoDBItem> _taskRepo;
        readonly ITasksService _taskService;
        readonly IPageDialogService _pageDialog;
        #endregion

        #region Commands
        public DelegateCommand EditTaskCommand { get; private set; }
        public DelegateCommand<TodoDBItem> TaskItemTappedCommand { get; private set; }
        public DelegateCommand AddTaskCommand { get; private set; }
        public DelegateCommand<string> DeleteTaskCommand { get; private set; }
        public DelegateCommand<string> StatusSelectCommand { get; private set; }
        public DelegateCommand<string> FilterCommand { get; private set; }
        public DelegateCommand UpdateTaskCommand { get; private set; }
        public DelegateCommand CloseUpdatePopupCommand { get; private set; }
        #endregion

        #region Properties
        private string _editTaskTitle;
        public string EditTaskTitle
        {
            get { return _editTaskTitle; }
            set { SetProperty(ref _editTaskTitle, value); }
        }

        private List<TodoDBItem> _taskList;
        public List<TodoDBItem> TaskList
        {
            get { return _taskList; }
            set { SetProperty(ref _taskList, value);
				if (_taskList.Count > 0)
					TaskListVisibility = true;
				else
					TaskListVisibility = false;
			}
        }

        private string _taskTitle;
        public string TaskTitle
        {
            get { return _taskTitle; }
            set { SetProperty(ref _taskTitle, value); }
        }

        private bool _deleteButtonVisibility;
        public bool DeleteButtonVisibility
        {
            get { return _deleteButtonVisibility; }
            set { SetProperty(ref _deleteButtonVisibility, value); }
        }

        private TodoItem _selectedItem;
        public TodoItem SelectedItem
        {
            get { return _selectedItem; }
            set { SetProperty(ref _selectedItem, value); }
        }

        private string _updatedTaskName;
        public string UpdatedTaskName
        {
            get { return _updatedTaskName; }
            set { SetProperty(ref _updatedTaskName, value); }
        }

        private bool _updatePopupVisibility;
        public bool UpdatePopupVisibility
        {
            get { return _updatePopupVisibility; }
            set { SetProperty(ref _updatePopupVisibility, value); }
        }

        private Color _waitingTabColor;
        public Color WaitingTabColor
        {
            get { return _waitingTabColor; }
            set { SetProperty(ref _waitingTabColor, value); }
        }

        private Color _inProgressTabColor;
        public Color InProgressTabColor
        {
            get { return _inProgressTabColor; }
            set { SetProperty(ref _inProgressTabColor, value); }
        }

        private Color _completedTabColor;
        public Color CompletedTabColor
        {
            get { return _completedTabColor; }
            set { SetProperty(ref _completedTabColor, value); }
        }

		private bool _taskListVisibility;
		public bool TaskListVisibility
		{
			get { return _taskListVisibility; }
			set { SetProperty(ref _taskListVisibility, value); }
		}
		

		private List<string> _filterList;
        #endregion

        public TaskHomePageViewModel(INavigationService navigationService, IPageDialogService dialogService)
            : base(navigationService)
        {
            IsLoading = true;
            Title = AppConstants.HomePageTitle;
            EditTaskTitle = AppConstants.HomePageEditTaskBtnTitle;

            _taskRepo = IocContainer.Instance.Container.Resolve<IRepository<TodoDBItem>>();
            _taskService = IocContainer.Instance.Container.Resolve<ITasksService>();
            _pageDialog = dialogService;

            EditTaskCommand = new DelegateCommand(ExecuteEditTaskCommand);
            TaskItemTappedCommand = new DelegateCommand<TodoDBItem>(ExecuteTaskItemTappedCommand);
            AddTaskCommand = new DelegateCommand(ExecuteAddTaskCommandAsync);
            DeleteTaskCommand = new DelegateCommand<string>(ExecuteDeleteTaskCommandAsync);
            StatusSelectCommand = new DelegateCommand<string>(ExecuteStatusSelectCommand);
            UpdateTaskCommand = new DelegateCommand(ExecuteUpdateTaskCommandAsync);
            CloseUpdatePopupCommand = new DelegateCommand(ExecuteCloseUpdatePopupCommand);
            FilterCommand = new DelegateCommand<string>(ExecuteFilterCommand);

            _filterList = new List<string>();

            WaitingTabColor = InProgressTabColor = CompletedTabColor = Color.White;

        }

		#region Public methods
		//Gets the Task List and Save to DB
        public async Task GetTaskListAsync()
        {
            var dbTaskList = _taskRepo.Get();
            if (IocContainer.Instance.Container.Resolve<IPlatformService>().IsConnected())
            {
				var result = await _taskService.GetTaskListAsync();
                IsLoading = false;
                var taskList = new List<TodoDBItem>();
                if (result?.Count > 0)
                {
                    await _taskRepo.DeleteAll();
                    foreach (var item in result)
                    {
                        var task = new TodoDBItem() { Id = item.Id, Name = item.Name, Status = item.Status };
                        if (!string.IsNullOrEmpty(task?.Id))
                        {
                            await _taskRepo.Insert(task);
                        }
                    } 
                    TaskList = _taskRepo.Get();
                }
            }
            else if (dbTaskList?.Count > 0)
            {
                TaskList = dbTaskList;
                IsLoading = false;
            }
		}
        #endregion

        #region Private Methods
        /// <summary>
        // Adds New Task
		/// </summary>
		private async void ExecuteAddTaskCommandAsync()
        {
            if (IocContainer.Instance.Container.Resolve<IPlatformService>().IsConnected())
            {
                if (!string.IsNullOrEmpty(TaskTitle?.Trim()))
                {
                    IsLoading = true;
                    var task = new TodoItem { Id = Guid.NewGuid().ToString(), Name = TaskTitle, Status = "Waiting" };
					var result = await _taskService.SaveTaskAsync(task);
					if (result != null)
                    {
                        var item = new TodoDBItem() { Id = result.Id, Name = result.Name, Status = result.Status };
                        await _taskRepo.Insert(item);
                        TaskList = _taskRepo.Get();
                        TaskTitle = string.Empty;
                        ExecuteFilterCommand(string.Empty);
                    }
                    IsLoading = false;
                }
                else
                    await _pageDialog.DisplayAlertAsync(AppConstants.AppName, AppConstants.AlertEmptyFieldInfo, AppConstants.AlertOkBtnTitle);
            }
            else
                await _pageDialog.DisplayAlertAsync(AppConstants.AlertOfflineTitle, AppConstants.AlertOfflineInfo, AppConstants.AlertOkBtnTitle);
        }

        /// <summary>
        /// Handles Edit button tap
        /// </summary>
        private void ExecuteEditTaskCommand()
        {
            if (EditTaskTitle == AppConstants.HomePageEditTaskBtnTitle)
            {
                EditTaskTitle = AppConstants.HomePageCancelBtnTitle;
                DeleteButtonVisibility = true;
            }
            else
            {
                EditTaskTitle = AppConstants.HomePageEditTaskBtnTitle;
                DeleteButtonVisibility = false;
            }
        }

        /// <summary>
        /// Deletes the Task
        /// </summary>
        /// <param name="id"></param>
        private async void ExecuteDeleteTaskCommandAsync(string id)
        {

            if (IocContainer.Instance.Container.Resolve<IPlatformService>().IsConnected())
            {

                IsLoading = true;
                var result = await _taskService.DeleteTaskAsync(id);
				if (result == -1)
				{
					await _pageDialog.DisplayAlertAsync(AppConstants.AppName, AppConstants.AlertErrorInfo, AppConstants.AlertOkBtnTitle);
					return;
				}
                var task = TaskList.FirstOrDefault(x => x.Id == id);
                await _taskRepo.Delete(task);
                TaskList = _taskRepo.Get();
                ExecuteFilterCommand(string.Empty);
                IsLoading = false;
				
            }
            else
                await _pageDialog.DisplayAlertAsync(AppConstants.AlertOfflineTitle, AppConstants.AlertOfflineInfo, AppConstants.AlertOkBtnTitle);
        }

        /// <summary>
        /// Updates the Task
        /// </summary>
        private async void ExecuteUpdateTaskCommandAsync()
        {
            SelectedItem.Name = UpdatedTaskName;
            if (string.IsNullOrEmpty(UpdatedTaskName.Trim()))
            {
                await _pageDialog.DisplayAlertAsync(AppConstants.AppName, AppConstants.AlertEmptyFieldInfo, AppConstants.AlertOkBtnTitle);
                return;
            }
            if (IocContainer.Instance.Container.Resolve<IPlatformService>().IsConnected())
            {
                IsLoading = true;
				var result = await _taskService.UpdateTaskAsync(SelectedItem);
                if (result != null)
                {
                    var task = new TodoDBItem() { Id = result.Id, Name = result.Name, Status = result.Status };
                    await _taskRepo.Update(task);
                    TaskList = _taskRepo.Get();
                    CloseUpdatePopupCommand.Execute();
                    ExecuteFilterCommand(string.Empty);
                }
                IsLoading = false;
            }
            else
            {
                await _pageDialog.DisplayAlertAsync(AppConstants.AlertOfflineTitle, AppConstants.AlertOfflineInfo, AppConstants.AlertOkBtnTitle);
            }
        }

        /// <summary>
        /// Filter the Task list
        /// </summary>
        /// <param name="status"></param>
        private void ExecuteFilterCommand(string status)
        {
            var filteredTaskList = new List<TodoDBItem>();
            if (!string.IsNullOrEmpty(status))
            {
                if (!_filterList.Contains(status))
                    _filterList.Add(status);
                else
                    _filterList.Remove(status);
                switch (status)
                {
                    case AppConstants.WaitingStatus:
                        WaitingTabColor = ChangeFilterTabColor(WaitingTabColor);
                        break;
                    case AppConstants.InProgressStatus:
                        InProgressTabColor = ChangeFilterTabColor(InProgressTabColor);
                        break;
                    case AppConstants.CompletedStatus:
                        CompletedTabColor = ChangeFilterTabColor(CompletedTabColor);
                        break;
                }
                if (_filterList.Count > 0)
                {
                    var list = _taskRepo.Get();
                    foreach (var item in _filterList)
                        filteredTaskList.AddRange(list.Where(x => x.Status == item));
                    TaskList = filteredTaskList;
                }
                else
                    TaskList = _taskRepo.Get();
            }
            else
            {
                WaitingTabColor = Color.White;
                InProgressTabColor = Color.White;
                CompletedTabColor = Color.White;
                _filterList.Clear();
            }
        }

        /// <summary>
        /// Change Filter tab color while selection
        /// </summary>
        /// <param name="tabColor"></param>
        /// <returns></returns>
        private Color ChangeFilterTabColor(Color tabColor)
        {
            if (tabColor == Color.White)
                return Color.LightBlue;
            else
                return Color.White;
        }

        /// <summary>
        /// Opens the update task popup
        /// </summary>
        /// <param name="item"></param>
        private void ExecuteTaskItemTappedCommand(TodoDBItem item)
        {
            SelectedItem = new TodoItem() { Id = item.Id, Name = item.Name, Status = item.Status };
            UpdatedTaskName = item.Name;
            UpdatePopupVisibility = true;
        }

        /// <summary>
        /// Closes the update task popup
        /// </summary>
        private void ExecuteCloseUpdatePopupCommand()
        {
            UpdatePopupVisibility = false;
        }

        /// <summary>
        /// Sets the selected task status while updating
        /// </summary>
        /// <param name="status"></param>
        private void ExecuteStatusSelectCommand(string status)
        {
            SelectedItem.Status = status;
        }

        #endregion
    }
}
