using ProjectManagement.DataLayer;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ProjectManagement.BusinessLayer
{
    [ExcludeFromCodeCoverage]
    public class TaskManagementProcess : ITaskManagementProcess
    {
        private readonly IProjectManagementDataConnector _connector;
        public TaskManagementProcess() : this(ProjectManagementDataConnector.Instance) { }
        public TaskManagementProcess(IProjectManagementDataConnector connector)
        {
            _connector = connector;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public bool AddTask(Entities.Task task)
        {
            if (!_connector.GetAllTasks().Any(t => t.Name == task.Name && t.ProjectID == task.Project.Id))
            {
                _connector.AddTask(ConvertToDataTask(task));
                return true;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CompleteTask(int id)
        {
            if (_connector.GetTaskById(id) != null)
            {
                _connector.CompleteTask(id);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteTask(int id)
        {
            if (_connector.GetTaskById(id) != null)
            {
                _connector.DeleteTask(id);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Entities.Task> GetParentTasks()
        {
            return _connector.GetAllParentTasks().ToArray().Select(task => ConvertToEntityTask(task));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<Entities.Task> GetParentTasksByProjectId(int id)
        {
            return _connector.GetAllParentTasksByProjectId(id).ToArray().Select(task => ConvertToEntityTask(task));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Entities.Task> GetTasks()
        {
            return _connector.GetAllTasks().ToArray().Select(task => ConvertToEntityTask(task));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<Entities.Task> GetTasksByProjectId(int id)
        {
            return _connector.GetAllTasksByProjectId(id).ToArray().Select(task => ConvertToEntityTask(task));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Entities.Task GetTaskByTaskId(int id)
        {
            var task = _connector.GetTaskById(id);
            if (task != null)
                return ConvertToEntityTask(task);
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public bool UpdateTask(Entities.Task task)
        {
            if (_connector.GetTaskById(task.Id) != null)
            {
                _connector.UpdateTask(ConvertToDataTask(task));
                return true;
            }
            return false;
        }

        public Entities.Task ConvertToEntityTask(Task task)
        {
            return
                new Entities.Task
                {
                    Id = task.ID,
                    Name = task.Name,
                    StartDate = task.StartDate,
                    EndDate = task.EndDate,
                    IsCompleted = task.IsCompleted,
                    IsParent = task.IsParent,
                    Priority = task.Priority,
                    Owner = task.TaskOwner != null ? new UserManagementProcess().ConvertToEntityUser(task.TaskOwner) : null,
                    Project = task.Project != null ? new ProjectManagementProcess().ConvertToEntityProject(task.Project) : null,
                    Parent = task.ParentTask != null ? ConvertToEntityTask(task.ParentTask) : null
                };
        }

        public Task ConvertToDataTask(Entities.Task task)
        {
            return
                new Task
                {
                    ID = task.Id,
                    Name = task.Name,
                    StartDate = task.StartDate,
                    EndDate = task.EndDate,
                    IsCompleted = task.IsCompleted,
                    IsParent = task.IsParent,
                    Priority = task.Priority,
                    UserID = task.Owner != null && task.Owner.Id > 0 ? task.Owner.Id : (int?)null,
                    ProjectID = task.Project.Id,
                    ParentID = task.Parent != null && task.Parent.Id > 0 ? task.Parent.Id : (int?)null
                };
        }
    }
}
