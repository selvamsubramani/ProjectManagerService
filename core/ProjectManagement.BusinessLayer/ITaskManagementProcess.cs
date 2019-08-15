using ProjectManagement.Entities;
using System.Collections.Generic;

namespace ProjectManagement.BusinessLayer
{
    public interface ITaskManagementProcess
    {
        IEnumerable<Task> GetTasks();
        IEnumerable<Task> GetTasksByProjectId(int id);
        IEnumerable<Task> GetParentTasks();
        IEnumerable<Task> GetParentTasksByProjectId(int id);
        Task GetTaskByTaskId(int id);
        bool AddTask(Task task);
        bool UpdateTask(Task task);
        bool CompleteTask(int id);
        bool DeleteTask(int id);
    }
}
