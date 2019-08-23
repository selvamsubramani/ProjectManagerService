using ProjectManagement.BusinessLayer;
using ProjectManagement.Entities;
using ProjectManagement.Logging;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ProjectManagement.Service.Controllers
{
    public class TaskController : ApiController
    {
        private readonly ITaskManagementProcess _process;
        public TaskController() : this(new TaskManagementProcess()) { }
        public TaskController(ITaskManagementProcess process)
        {
            _process = process;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetAllTasks()
        {
            try
            {
                var result = _process.GetTasks();
                if (result.Any())
                    return Ok(result);
                LogHelper.LogWarn("No tasks");
                return ResponseMessage(new HttpResponseMessage(HttpStatusCode.NoContent));
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
                return InternalServerError(ex);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetAllParentTasks()
        {
            try
            {
                var result = _process.GetParentTasks();
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
        public IHttpActionResult GetAllTasksByProjectId(int id)
        {
            try
            {
                var result = _process.GetTasksByProjectId(id);
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
        public IHttpActionResult GetAllParentTasksByProjectId(int id)
        {
            try
            {
                var result = _process.GetParentTasksByProjectId(id);
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
        public IHttpActionResult GetTaskByTaskId(int id)
        {
            try
            {
                var result = _process.GetTaskByTaskId(id);
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
        /// <param name="task"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult CreateTask(Task task)
        {
            try
            {
                if (_process.AddTask(task))
                    return Created(new Uri(Request.RequestUri, $"GetTaskByTaskId/{task.Id}"), task);
                return BadRequest("Task is already available.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        [HttpPut]
        public IHttpActionResult UpdateTask(Task task)
        {
            try
            {
                if (_process.UpdateTask(task))
                    return ResponseMessage(new HttpResponseMessage(HttpStatusCode.Accepted));
                return BadRequest("Task is not found to update.");
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
        public IHttpActionResult CompleteTask(int id)
        {
            try
            {
                if (_process.CompleteTask(id))
                    return ResponseMessage(new HttpResponseMessage(HttpStatusCode.Accepted));
                return BadRequest("Task is not found or already completed.");
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
        public IHttpActionResult DeleteTask(int id)
        {
            try
            {
                if (_process.DeleteTask(id))
                    return ResponseMessage(new HttpResponseMessage(HttpStatusCode.Accepted));
                return BadRequest("Task is not found or already deleted.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
