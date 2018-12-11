using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Navigation;
using Prism.Services;
using System.Threading.Tasks;
using TaskManager.Helpers;
using TaskManager.Interfaces;
using TaskManager.Model;
using TaskManager.ViewModels;
using DryIoc;
using TaskManager.UnitTest.MockData;

namespace TaskManager.UnitTest
{
    [TestClass]
    public class HomePageViewModelTest
    {
        private Mock<INavigationService> _navigationMock;
        private Mock<IPageDialogService> _pageDialogServiceMock;
        private Mock<ITasksService> _taskServiceMock;
        private Mock<IRepository<TodoDBItem>> _dbMock;
        private Mock<IPlatformService> _platformServiceMock;
        private bool _alertCalled = false;
        private bool _emptyTaskTitleAlertCalled = false;
        private bool _errorAlertCalled = false;

        /// <summary>
        /// Initialize the test
        /// </summary>
        [TestInitialize]
        public void Init()
        {
            _navigationMock = new Mock<INavigationService>();
            _navigationMock.Setup(e => e.NavigateAsync("NavigationPage/TaskHomePage"));

            //Mocking No Network alert view
            _pageDialogServiceMock = new Mock<IPageDialogService>();
            _pageDialogServiceMock.Setup(method => method.DisplayAlertAsync(AppConstants.AlertOfflineTitle, AppConstants.AlertOfflineInfo, AppConstants.AlertOkBtnTitle)).Returns(() =>
            {
                _alertCalled = true;
                return Task.FromResult(0);
            });
            //Mocking Empty Task Title alert view
            _pageDialogServiceMock.Setup(method => method.DisplayAlertAsync(AppConstants.AppName, AppConstants.AlertEmptyFieldInfo, AppConstants.AlertOkBtnTitle)).Returns(() =>
            {
                _emptyTaskTitleAlertCalled = true;
                return Task.FromResult(0);
            });

            //Mocking Error alert view
            _pageDialogServiceMock.Setup(method => method.DisplayAlertAsync(AppConstants.AppName, AppConstants.AlertErrorInfo, AppConstants.AlertOkBtnTitle)).Returns(() =>
            {
                _errorAlertCalled = true;
                return Task.FromResult(0);
            });

            //Mocking Task Services
            _taskServiceMock = new Mock<ITasksService>();
            _taskServiceMock.Setup(method => method.DeleteTaskAsync("1")).Returns(() => { return Task.FromResult(1); });
            _taskServiceMock.Setup(method => method.GetTaskListAsync()).Returns(() => { return Task.FromResult(TaskMockData.ReturnServiceTaskList()); });
            _taskServiceMock.Setup(method => method.SaveTaskAsync(TaskMockData.ReturnServiceTaskList()[0])).Returns(() =>
            {
                return Task.FromResult(TaskMockData.ReturnServiceTaskList()[0]);
            });

            //Mocking Task DB Calls
            _dbMock = new Mock<IRepository<TodoDBItem>>();
            _dbMock.Setup(method => method.Get()).Returns(() => { return TaskMockData.ReturnDBTaskList(); });
            _dbMock.Setup(method => method.DeleteAll()).Returns(Task.FromResult(0));
            _dbMock.Setup(method => method.Delete(TaskMockData.ReturnDBTaskList()[0])).Returns(() => { return Task.FromResult(1); });

            //Mocking Network Connectivity value
            _platformServiceMock = new Mock<IPlatformService>();
            _platformServiceMock.Setup(method => method.IsConnected()).Returns(true);

            //Mocking Container instance
            IocContainer.Instance.Container.UseInstance<IPlatformService>(_platformServiceMock.Object);
            IocContainer.Instance.Container.UseInstance<ITasksService>(_taskServiceMock.Object);
            IocContainer.Instance.Container.UseInstance<IRepository<TodoDBItem>>(_dbMock.Object);

        }

        #region Tests

        /// <summary>
        /// This test check the object creation and verifies the intial data required for the object.
        /// </summary>
        [TestMethod]
        public void HomePageViewModelInitializationTest()
        {
            TaskHomePageViewModel viewModel = new TaskHomePageViewModel(_navigationMock.Object, _pageDialogServiceMock.Object);
            Assert.IsTrue(viewModel.IsLoading, "Error Happened when loading");
            Assert.AreEqual(viewModel.Title.ToString(), AppConstants.HomePageTitle.ToString(), "TODO::Message");
            Assert.IsTrue(Xamarin.Forms.Color.White.Equals(viewModel.WaitingTabColor), "Error in initialization, Initialization not Completed");
            Assert.IsTrue(Xamarin.Forms.Color.White.Equals(viewModel.InProgressTabColor), "Error in initialization, Initialization not Completed");
            Assert.IsTrue(Xamarin.Forms.Color.White.Equals(viewModel.CompletedTabColor), "Error in initialization, Initialization not Completed");
        }

        /// <summary>
        /// This method test GetTaskListAsync method.
        /// </summary>
        [TestMethod]
        public async Task HomePageViewModel_GetTaskListAsync_Test()
        {
            TaskHomePageViewModel viewModel = new TaskHomePageViewModel(_navigationMock.Object, _pageDialogServiceMock.Object);
            _platformServiceMock.Setup(method => method.IsConnected()).Returns(true);
            await viewModel.GetTaskListAsync();

            Assert.AreEqual<int>(viewModel.TaskList.Count, TaskMockData.ReturnServiceTaskList().Count);
        }

        /// <summary>
        /// This method test GetTaskListAsync while no internet connectivity.
        /// </summary>
        [TestMethod]
        public async Task HomePageViewModel_GetTaskListAsync_NoConnecteciont_Test()
        {


            TaskHomePageViewModel viewModel = new TaskHomePageViewModel(_navigationMock.Object, _pageDialogServiceMock.Object);
            _platformServiceMock.Setup(method => method.IsConnected()).Returns(false);
            await viewModel.GetTaskListAsync();

            Assert.AreEqual<int>(viewModel.TaskList.Count, TaskMockData.ReturnServiceTaskList().Count);
        }

        /// <summary>
        /// This method test delete task method.
        /// </summary>
        [TestMethod]
        public async Task HomePageViewModel_DeleteTask_SuccessTest()
        {
            var viewModel = new TaskHomePageViewModel(_navigationMock.Object, _pageDialogServiceMock.Object);
            viewModel.TaskList = TaskMockData.ReturnDBTaskList();
            _platformServiceMock.Setup(method => method.IsConnected()).Returns(true);
            await Task.Run(() =>
            {
                viewModel.DeleteTaskCommand.Execute("1");
            });

            Assert.IsFalse(_errorAlertCalled, "Delete Task Failed");
        }

        /// <summary>
        /// This method test AddTask emethod with empty Task Title.
        /// </summary>
        [TestMethod]
        public void HomePageViewModel_AddTask_EmptyTaskTitleTest()
        {
            TaskHomePageViewModel viewModel = new TaskHomePageViewModel(_navigationMock.Object, _pageDialogServiceMock.Object);
            _platformServiceMock.Setup(method => method.IsConnected()).Returns(true);
            viewModel.AddTaskCommand.Execute();

            Assert.IsTrue(_emptyTaskTitleAlertCalled, "Empty alert failed to call");
        }

        /// <summary>
        /// This method Test AddTask method while no internet connectivity
        /// </summary>
        [TestMethod]
        public void HomePageViewModel_AddTask_NoConnection_Test()
        {
            TaskHomePageViewModel viewModel = new TaskHomePageViewModel(_navigationMock.Object, _pageDialogServiceMock.Object);
            _platformServiceMock.Setup(method => method.IsConnected()).Returns(false);
            viewModel.AddTaskCommand.Execute();

            Assert.IsTrue(_alertCalled, "alert Failed to call");
        }

        /// <summary>
        /// This method test the update popup visibility.
        /// </summary>
        [TestMethod]
        public void HomePageViewModel_UpdatePopupVisibilityTest()
        {
            TaskHomePageViewModel viewModel = new TaskHomePageViewModel(_navigationMock.Object, _pageDialogServiceMock.Object);
            viewModel.UpdatePopupVisibility = false;
            viewModel.TaskItemTappedCommand.Execute(TaskMockData.ReturnDBTaskList()[0]);

            Assert.IsTrue(viewModel.UpdatePopupVisibility, "Popup not visible");
        }

        /// <summary>
        /// This method test the ClosePopupCommand execution
        /// </summary>
        [TestMethod]
        public void HomePageViewModel_CloseUpdatePopupVisibilityTest()
        {
            TaskHomePageViewModel viewModel = new TaskHomePageViewModel(_navigationMock.Object, _pageDialogServiceMock.Object);
            viewModel.UpdatePopupVisibility = true;
            viewModel.CloseUpdatePopupCommand.Execute();

            Assert.IsFalse(viewModel.UpdatePopupVisibility, "Popup visible");
        }

        #endregion
    }
}
