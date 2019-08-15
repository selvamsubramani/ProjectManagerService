using System.Linq;

namespace ProjectManagement.DataLayer
{
    public interface IProjectManagementDataConnector
    {
        IQueryable<User> GetAllUsers();
        User GetUserById(int id);
        void AddUser(User user);
        void UpdateUser(User user);
        void DeleteUser(int id);


        IQueryable<Project> GetAllProjects();
        Project GetProjectById(int id);
        void AddProject(Project project);
        void UpdateProject(Project project);
        void SuspendProject(int id);
        void DeleteProject(int id);


        IQueryable<Task> GetAllTasks();
        IQueryable<Task> GetAllTasksByProjectId(int id);
        IQueryable<Task> GetAllParentTasks();
        IQueryable<Task> GetAllParentTasksByProjectId(int id);
        Task GetTaskById(int id);
        void AddTask(Task task);
        void UpdateTask(Task task);
        void CompleteTask(int id);
        void DeleteTask(int id);
    }
}
