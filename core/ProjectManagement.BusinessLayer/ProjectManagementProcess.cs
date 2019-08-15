using ProjectManagement.DataLayer;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ProjectManagement.BusinessLayer
{
    [ExcludeFromCodeCoverage]
    public class ProjectManagementProcess : IProjectManagementProcess
    {
        private readonly IProjectManagementDataConnector _connector;
        public ProjectManagementProcess() : this(ProjectManagementDataConnector.Instance) { }
        public ProjectManagementProcess(IProjectManagementDataConnector connector)
        {
            _connector = connector;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Entities.Project> GetProjects()
        {
            return _connector.GetAllProjects().ToArray().Select(project => ConvertToEntityProject(project));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Entities.Project GetProjectByProjectId(int id)
        {
            var project = _connector.GetProjectById(id);
            if (project != null)
                return ConvertToEntityProject(project);
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public bool AddProject(Entities.Project project)
        {
            if (!_connector.GetAllProjects().Any(p => p.Name == project.Name))
            {
                _connector.AddProject(ConvertToDataProject(project));
                return true;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public bool UpdateProject(Entities.Project project)
        {
            if (_connector.GetProjectById(project.Id) != null)
            {
                _connector.UpdateProject(ConvertToDataProject(project));
                return true;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool SuspendProject(int id)
        {
            if (_connector.GetProjectById(id) != null)
            {
                _connector.SuspendProject(id);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteProject(int id)
        {
            if (_connector.GetProjectById(id) != null)
            {
                _connector.DeleteProject(id);
                return true;
            }
            return false;
        }


        public Entities.Project ConvertToEntityProject(Project project)
        {
            return
                new Entities.Project
                {
                    Id = project.ID,
                    Name = project.Name,
                    StartDate = project.StartDate.Value,
                    EndDate = project.EndDate.Value,
                    IsSuspended = project.IsSuspended,
                    Priority = project.Priority,
                    Manager = new UserManagementProcess().ConvertToEntityUser(project.Manager)
                };
        }

        public Project ConvertToDataProject(Entities.Project project)
        {
            return
                new Project
                {
                    ID = project.Id,
                    Name = project.Name,
                    StartDate = project.StartDate,
                    EndDate = project.EndDate,
                    IsSuspended = project.IsSuspended,
                    Priority = project.Priority,
                    Manager = new UserManagementProcess().ConvertToDataUser(project.Manager)
                };
        }
    }
}
