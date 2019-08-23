using System;
using System.Linq;

namespace ProjectManagement.DataLayer
{
    public class ProjectManagementDataConnector : IProjectManagementDataConnector
    {
        #region Connector Instance
        private readonly ProjectManagementDataModel _model;
        public ProjectManagementDataConnector(ProjectManagementDataModel model)
        {
            _model = model;
        }

        private static readonly Lazy<ProjectManagementDataConnector> lazy = new Lazy<ProjectManagementDataConnector>(() => new ProjectManagementDataConnector(new ProjectManagementDataModel(false)));
        public static ProjectManagementDataConnector Instance
        {
            get
            {
                return lazy.Value;
            }
        }
        #endregion

        #region Users
        public IQueryable<User> GetAllUsers()
        {
            return _model.Users;
        }

        public User GetUserById(int id)
        {
            return GetAllUsers().FirstOrDefault(u => u.ID == id);
        }

        public void AddUser(User user)
        {
            _model.Users.Add(user);
            _model.SaveChanges();
        }

        public void UpdateUser(User user)
        {
            var userToUpdate = GetUserById(user.ID);
            userToUpdate.FirstName = user.FirstName;
            userToUpdate.LastName = user.LastName;
            userToUpdate.EmployeeID = user.EmployeeID;
            _model.SaveChanges();
        }

        public void DeleteUser(int id)
        {
            var userToDelete = GetUserById(id);
            _model.Users.Remove(userToDelete);
            _model.SaveChanges();
        }
        #endregion

        #region Projects
        public IQueryable<Project> GetAllProjects()
        {
            return _model.Projects;
        }

        public Project GetProjectById(int id)
        {
            return GetAllProjects().FirstOrDefault(p => p.ID == id);
        }

        public void AddProject(Project project)
        {
            _model.Projects.Add(project);
            _model.SaveChanges();
        }

        public void UpdateProject(Project project)
        {
            var projectToUpdate = GetProjectById(project.ID);
            projectToUpdate.Name = project.Name;
            projectToUpdate.StartDate = project.StartDate;
            projectToUpdate.EndDate = project.EndDate;
            projectToUpdate.Priority = project.Priority;
            projectToUpdate.ManagerID = project.ManagerID;
            _model.SaveChanges();
        }
        public void SuspendProject(int id)
        {
            var projectToSuspend = GetProjectById(id);
            projectToSuspend.IsSuspended = true;
            _model.SaveChanges();
        }

        public void DeleteProject(int id)
        {
            var projectToDelete = GetProjectById(id);
            _model.Projects.Remove(projectToDelete);
            _model.SaveChanges();
        }
        #endregion

        #region Tasks
        public IQueryable<Task> GetAllTasks()
        {
            return _model.Tasks;
        }

        public IQueryable<Task> GetAllTasksByProjectId(int id)
        {
            return _model.Tasks.Where(t => t.ProjectID == id && !t.IsParent);
        }

        public IQueryable<Task> GetAllParentTasks()
        {
            return _model.Tasks.Where(t => t.IsParent);
        }

        public IQueryable<Task> GetAllParentTasksByProjectId(int id)
        {
            return _model.Tasks.Where(t => t.ProjectID == id && t.IsParent);
        }

        public Task GetTaskById(int id)
        {
            return GetAllTasks().FirstOrDefault(t => t.ID == id);
        }

        public void AddTask(Task task)
        {
            _model.Tasks.Add(task);
            _model.SaveChanges();
        }

        public void UpdateTask(Task task)
        {
            var taskToUpdate = GetTaskById(task.ID);
            taskToUpdate.Name = task.Name;
            taskToUpdate.ParentID = task.ParentID;
            taskToUpdate.Priority = task.Priority;
            taskToUpdate.StartDate = task.StartDate;
            taskToUpdate.EndDate = task.EndDate;
            taskToUpdate.UserID = task.UserID;
            _model.SaveChanges();
        }

        public void CompleteTask(int id)
        {
            var taskToComplete = GetTaskById(id);
            taskToComplete.IsCompleted = true;
            taskToComplete.EndDate = DateTime.Now;
            _model.SaveChanges();
        }

        public void DeleteTask(int id)
        {
            var taskToDelete = GetTaskById(id);
            _model.Tasks.Remove(taskToDelete);
            _model.SaveChanges();
        }
        #endregion
    }
}
