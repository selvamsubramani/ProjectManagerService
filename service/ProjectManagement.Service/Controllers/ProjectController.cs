using ProjectManagement.BusinessLayer;
using ProjectManagement.Entities;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ProjectManagement.Service.Controllers
{
    public class ProjectController : ApiController
    {
        private readonly IProjectManagementProcess _process;
        public ProjectController() : this(new ProjectManagementProcess()) { }
        public ProjectController(IProjectManagementProcess process)
        {
            _process = process;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetAllProjects()
        {
            try
            {
                var result = _process.GetProjects();
                if (result.Any())
                    return Ok(result);
                return ResponseMessage(new HttpResponseMessage(HttpStatusCode.NoContent));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetProjectById(int id)
        {
            try
            {
                var result = _process.GetProjectByProjectId(id);
                if (result != null)
                    return Ok(result);
                return NotFound();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult CreateProject(Project project)
        {
            try
            {
                if (_process.AddProject(project))
                    return Created(new Uri(Request.RequestUri, $"GetProjectById/{project.Id}"), project);
                return BadRequest("Project is already available.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        [HttpPut]
        public IHttpActionResult UpdateProject(Project project)
        {
            try
            {
                if (_process.UpdateProject(project))
                    return ResponseMessage(new HttpResponseMessage(HttpStatusCode.Accepted));
                return BadRequest("Project is not found to update.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        public IHttpActionResult SuspendProject(int id)
        {
            try
            {
                if (_process.SuspendProject(id))
                    return ResponseMessage(new HttpResponseMessage(HttpStatusCode.Accepted));
                return BadRequest("Project is not found or already suspended.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public IHttpActionResult DeleteProject(int id)
        {
            try
            {
                if (_process.DeleteProject(id))
                    return ResponseMessage(new HttpResponseMessage(HttpStatusCode.Accepted));
                return BadRequest("Project is not found or already deleted.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
