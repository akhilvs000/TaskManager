using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Interfaces;
using TaskManager.Model;

namespace TaskManager.Services
{
    public class TasksService : ITasksService
    {
        private RestClient client;

        public TasksService()
        {
            client = new RestClient(AppConstants.TasksServiceURL);
			client.AddDefaultHeader("ZUMO-API-VERSION", "2.0.0");
		}

        public Task<int> DeleteTaskAsync(string id)
        {
            try
            {
                var taskCompletionSource = new TaskCompletionSource<int>();
                RestRequest request = new RestRequest("tables/todoitem/" + id, Method.DELETE);
                client.ExecuteAsync<int>(request, (response) => taskCompletionSource.SetResult(response.Data));
                return taskCompletionSource.Task;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Task.FromResult(-1);
            }
        }

        public Task<List<TodoItem>> GetTaskListAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<List<TodoItem>>();
            RestRequest request = new RestRequest("tables/todoitem", Method.GET);
            client.ExecuteAsync<List<TodoItem>>(request, (response) => taskCompletionSource.SetResult(response.Data));
            return taskCompletionSource.Task;
        }

        public Task<TodoItem> SaveTaskAsync(TodoItem todoItem)
        {
            try
            {
                var taskCompletionSource = new TaskCompletionSource<TodoItem>();
                RestRequest request = new RestRequest("tables/todoitem/", Method.POST);
                request.AddJsonBody(todoItem);
                client.ExecuteAsync<TodoItem>(request, (response) => taskCompletionSource.SetResult(response.Data));
                return taskCompletionSource.Task;
            }
            catch(Exception ex)
            {
				Console.WriteLine(ex.Message);
                return null;
            }
        }

        public Task<TodoItem> UpdateTaskAsync(TodoItem todoItem)
        {
            try
            {
                var taskCompletionSource = new TaskCompletionSource<TodoItem>();
                RestRequest request = new RestRequest("tables/todoitem/" + todoItem.Id, Method.PATCH);
                request.AddJsonBody(todoItem);
                client.ExecuteAsync<TodoItem>(request, (response) => taskCompletionSource.SetResult(response.Data));

                return taskCompletionSource.Task;
            }
            catch (Exception ex)
            {
				Console.WriteLine(ex.Message);
				return null;
            }
        }
    }
}
