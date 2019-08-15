using ProjectManagement.Entities;
using System.Collections.Generic;

namespace ProjectManagement.BusinessLayer
{
    public interface IProjectManagementProcess
    {
        IEnumerable<Project> GetProjects();
        Project GetProjectByProjectId(int id);
        bool AddProject(Project project);
        bool UpdateProject(Project project);
        bool SuspendProject(int id);
        bool DeleteProject(int id);
    }
}
