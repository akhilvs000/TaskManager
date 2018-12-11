using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManager.Model;

namespace TaskManager.Interfaces
{
    public interface ITasksService
    {
        Task<List<TodoItem>> GetTaskListAsync();

        Task<TodoItem> SaveTaskAsync(TodoItem task);

        Task<int> DeleteTaskAsync(string id);

        Task<TodoItem> UpdateTaskAsync(TodoItem task);
    }
}
