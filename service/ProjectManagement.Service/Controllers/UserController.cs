using ProjectManagement.BusinessLayer;
using ProjectManagement.Entities;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ProjectManagement.Logging;

namespace ProjectManagement.Service.Controllers
{
    public class UserController : ApiController
    {
        private readonly IUserManagementProcess _process;
        public UserController() : this(new UserManagementProcess()) { }
        public UserController(IUserManagementProcess process)
        {
            _process = process;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetAllUsers()
        {
            try
            {
                var result = _process.GetUsers();
                if (result.Any())
                    return Ok(result);
                LogHelper.LogWarn("No users");
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
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetUserById(int id)
        {
            try
            {
                var result = _process.GetUserByUserId(id);
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
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult CreateUser(User user)
        {
            try
            {
                if (_process.AddUser(user))
                    return Created(new Uri(Request.RequestUri, $"GetUserById/{user.Id}"), user);
                return BadRequest("User is already available.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut]
        public IHttpActionResult UpdateUser(User user)
        {
            try
            {
                if (_process.UpdateUser(user))
                    return ResponseMessage(new HttpResponseMessage(HttpStatusCode.Accepted));
                return BadRequest("User is not found to update.");
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
        public IHttpActionResult DeleteUser(int id)
        {
            try
            {
                if (_process.DeleteUser(id))
                    return ResponseMessage(new HttpResponseMessage(HttpStatusCode.Accepted));
                return BadRequest("User is not found or already deleted.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
