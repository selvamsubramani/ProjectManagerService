using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ProjectManagement.BusinessLayer;
using ProjectManagement.Entities;
using ProjectManagement.Service.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Results;

namespace ProjectManagement.Test
{
    [TestClass]
    public class ServiceTests
    {
        IEnumerable<User> users;
        IEnumerable<Project> projects;
        IEnumerable<Task> tasks;

        [TestInitialize]
        public void Setup()
        {
            users = new User[]
            {
                new User { Id = 1, FirstName = "First-01", LastName = "Last-01", EmployeeId = 1001 },
                new User { Id = 2, FirstName = "First-02", LastName = "Last-02", EmployeeId = 1002 },
                new User { Id = 3, FirstName = "First-03", LastName = "Last-03", EmployeeId = 1003 }
            };

            projects = new Project[]
            {
                new Project { Id = 1, Name = "Project-01", Priority = 5, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(2), IsSuspended = false, Manager = users.First(x => x.Id == 1) },
                new Project { Id = 2, Name = "Project-02", Priority = 15, StartDate = DateTime.Today.AddDays(5), EndDate = DateTime.Today.AddDays(7), IsSuspended = false, Manager = users.First(x => x.Id == 2) },
                new Project { Id = 3, Name = "Project-03", Priority = 25, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(2), IsSuspended = false, Manager = users.First(x => x.Id == 3) }
            };

            Task parent = new Task { Id = 1, Name = "Task-01", StartDate = DateTime.MinValue, EndDate = DateTime.MaxValue, Priority = 5, IsCompleted = false, IsParent = true, Project = projects.First() };
            tasks = new Task[]
            {
                parent,
                new Task { Id = 2, Name = "Task-02", StartDate = DateTime.MinValue, EndDate = DateTime.MaxValue, Priority = 10, Parent = parent, IsCompleted = false, IsParent = false, Owner = users.First(u => u.Id == 2), Project = projects.First(p => p.Id == 1) },
                new Task { Id = 3, Name = "Task-03", StartDate = DateTime.MinValue, EndDate = DateTime.MaxValue, Priority = 15, Parent = parent, IsCompleted = false, IsParent = false, Owner = users.First(u => u.Id == 3), Project = projects.First(p => p.Id == 2) }
            };
        }

        #region User

        #region GetAllUsers
        [TestMethod]
        public void ShouldGetAllUsers()
        {
            var process = new Mock<IUserManagementProcess>();
            process.Setup(m => m.GetUsers()).Returns(users);
            var controller = new UserController(process.Object);
            var output = controller.GetAllUsers();
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(OkNegotiatedContentResult<IEnumerable<User>>));
            var result = output as OkNegotiatedContentResult<IEnumerable<User>>;
            Assert.AreEqual(users.Count(), result.Content.Count());
        }

        [TestMethod]
        public void ShouldGetNoUsers()
        {
            var process = new Mock<IUserManagementProcess>();
            var input = new User[] { };
            process.Setup(m => m.GetUsers()).Returns(input.AsQueryable());
            var controller = new UserController(process.Object);
            var output = controller.GetAllUsers();
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(ResponseMessageResult));
            var result = output as ResponseMessageResult;
            Assert.IsNull(result.Response.Content);
            Assert.AreEqual(System.Net.HttpStatusCode.NoContent, result.Response.StatusCode);
        }
        [TestMethod]
        public void ShouldGetErrorOnGetAllUsers()
        {
            var process = new Mock<IUserManagementProcess>();
            IQueryable<User> input = null;
            process.Setup(m => m.GetUsers()).Returns(input);
            var controller = new UserController(process.Object);
            var output = controller.GetAllUsers();
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(ExceptionResult));
            var result = output as ExceptionResult;
            Assert.AreEqual("Value cannot be null.\r\nParameter name: source", result.Exception.Message);
        }
        #endregion

        #region GetUserById
        [TestMethod]
        public void ShouldGetUserById()
        {
            var process = new Mock<IUserManagementProcess>();
            var user = users.FirstOrDefault(p => p.Id == 1);
            process.Setup(m => m.GetUserByUserId(It.IsAny<int>())).Returns(user);
            var controller = new UserController(process.Object);
            var output = controller.GetUserById(1);
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(OkNegotiatedContentResult<User>));
            var result = output as OkNegotiatedContentResult<User>;
            Assert.AreEqual(user.Id, result.Content.Id);
        }

        [TestMethod]
        public void ShouldGetNoUserById()
        {
            var process = new Mock<IUserManagementProcess>();
            var user = users.FirstOrDefault(u => u.Id == 0);
            process.Setup(m => m.GetUserByUserId(It.IsAny<int>())).Returns(user);
            var controller = new UserController(process.Object);
            var output = controller.GetUserById(0);
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(NotFoundResult));
        }
        [TestMethod]
        public void ShouldGetErrorOnGetUserById()
        {
            var process = new Mock<IUserManagementProcess>();
            process.Setup(m => m.GetUserByUserId(It.IsAny<int>())).Throws(new Exception("Server error"));
            var controller = new UserController(process.Object);
            var output = controller.GetUserById(0);
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(ExceptionResult));
            var result = output as ExceptionResult;
            Assert.AreEqual("Server error", result.Exception.Message);
        }
        #endregion

        #region AddUser
        [TestMethod]
        public void ShouldCreateUser()
        {
            var userId = new Random().Next(2, 100);
            var process = new Mock<IUserManagementProcess>();
            process.Setup(m => m.AddUser(It.IsAny<User>())).Returns<User>(u =>
            {
                u.Id = userId;
                return true;
            });
            var controller = new UserController(process.Object);
            controller.Request = new System.Net.Http.HttpRequestMessage
            {
                RequestUri = new Uri("http://localhost/api/createuser"),
                Method = System.Net.Http.HttpMethod.Post
            };
            var output = controller.CreateUser(new User { });
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(CreatedNegotiatedContentResult<User>));
            var result = output as CreatedNegotiatedContentResult<User>;
            Assert.AreEqual(userId, result.Content.Id);
        }

        [TestMethod]
        public void ShouldNotCreateUser()
        {
            var process = new Mock<IUserManagementProcess>();
            process.Setup(m => m.AddUser(It.IsAny<User>())).Returns(false);
            var controller = new UserController(process.Object);
            controller.Request = new System.Net.Http.HttpRequestMessage
            {
                RequestUri = new Uri("http://localhost/api/createuser"),
                Method = System.Net.Http.HttpMethod.Post
            };
            var output = controller.CreateUser(new User { });
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public void ShouldGetErrorOnCreateUser()
        {
            var process = new Mock<IUserManagementProcess>();
            process.Setup(m => m.AddUser(It.IsAny<User>())).Throws(new Exception("Internal Error"));
            var controller = new UserController(process.Object);
            var output = controller.CreateUser(new User { });
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(ExceptionResult));
            var result = output as ExceptionResult;
            Assert.AreEqual("Internal Error", result.Exception.Message);
        }
        #endregion

        #region UpdateUser
        [TestMethod]
        public void ShouldUpdateUser()
        {
            var process = new Mock<IUserManagementProcess>();
            process.Setup(m => m.UpdateUser(It.IsAny<User>())).Returns(true);
            var controller = new UserController(process.Object);
            var output = controller.UpdateUser(new User { });
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(ResponseMessageResult));
            var result = output as ResponseMessageResult;
            Assert.IsNull(result.Response.Content);
            Assert.AreEqual(System.Net.HttpStatusCode.Accepted, result.Response.StatusCode);
        }
        [TestMethod]
        public void ShouldNotUpdateUser()
        {
            var process = new Mock<IUserManagementProcess>();
            process.Setup(m => m.UpdateUser(It.IsAny<User>())).Returns(false);
            var controller = new UserController(process.Object);
            var output = controller.UpdateUser(new User { });
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(BadRequestErrorMessageResult));
        }
        [TestMethod]
        public void ShouldGetErrorOnUpdateUser()
        {
            var process = new Mock<IUserManagementProcess>();
            process.Setup(m => m.UpdateUser(It.IsAny<User>())).Throws(new Exception("Internal Error"));
            var controller = new UserController(process.Object);
            var output = controller.UpdateUser(new User { });
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(ExceptionResult));
            var result = output as ExceptionResult;
            Assert.AreEqual("Internal Error", result.Exception.Message);
        }
        #endregion        

        #region DeleteUser
        [TestMethod]
        public void ShouldDeleteUser()
        {
            var process = new Mock<IUserManagementProcess>();
            process.Setup(m => m.DeleteUser(It.IsAny<int>())).Returns(true);
            var controller = new UserController(process.Object);
            var output = controller.DeleteUser(1);
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(ResponseMessageResult));
            var result = output as ResponseMessageResult;
            Assert.IsNull(result.Response.Content);
            Assert.AreEqual(System.Net.HttpStatusCode.Accepted, result.Response.StatusCode);
        }

        [TestMethod]
        public void ShouldNotDeleteUser()
        {
            var process = new Mock<IUserManagementProcess>();
            process.Setup(m => m.DeleteUser(It.IsAny<int>())).Returns(false);
            var controller = new UserController(process.Object);
            var output = controller.DeleteUser(1);
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public void ShouldGetErrorOnDeleteUser()
        {
            var process = new Mock<IUserManagementProcess>();
            process.Setup(m => m.DeleteUser(It.IsAny<int>())).Throws(new Exception("Internal Error"));
            var controller = new UserController(process.Object);
            var output = controller.DeleteUser(1);
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(ExceptionResult));
            var result = output as ExceptionResult;
            Assert.AreEqual("Internal Error", result.Exception.Message);
        }
        #endregion

        #endregion

        #region Project

        #region GetAllProjects
        [TestMethod]
        public void ShouldGetAllProjects()
        {
            var process = new Mock<IProjectManagementProcess>();
            process.Setup(m => m.GetProjects()).Returns(projects);
            var controller = new ProjectController(process.Object);
            var output = controller.GetAllProjects();
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(OkNegotiatedContentResult<IEnumerable<Project>>));
            var result = output as OkNegotiatedContentResult<IEnumerable<Project>>;
            Assert.AreEqual(projects.Count(), result.Content.Count());
        }

        [TestMethod]
        public void ShouldGetNoProjects()
        {
            var process = new Mock<IProjectManagementProcess>();
            var input = new Project[] { };
            process.Setup(m => m.GetProjects()).Returns(input.AsQueryable());
            var controller = new ProjectController(process.Object);
            var output = controller.GetAllProjects();
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(ResponseMessageResult));
            var result = output as ResponseMessageResult;
            Assert.IsNull(result.Response.Content);
            Assert.AreEqual(System.Net.HttpStatusCode.NoContent, result.Response.StatusCode);
        }
        [TestMethod]
        public void ShouldGetErrorOnGetAllProjects()
        {
            var process = new Mock<IProjectManagementProcess>();
            IQueryable<Project> input = null;
            process.Setup(m => m.GetProjects()).Returns(input);
            var controller = new ProjectController(process.Object);
            var output = controller.GetAllProjects();
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(ExceptionResult));
            var result = output as ExceptionResult;
            Assert.AreEqual("Value cannot be null.\r\nParameter name: source", result.Exception.Message);
        }
        #endregion

        #region GetProjectById
        [TestMethod]
        public void ShouldGetProjectById()
        {
            var process = new Mock<IProjectManagementProcess>();
            var project = projects.FirstOrDefault(p => p.Id == 1);
            process.Setup(m => m.GetProjectByProjectId(It.IsAny<int>())).Returns(project);
            var controller = new ProjectController(process.Object);
            var output = controller.GetProjectById(1);
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(OkNegotiatedContentResult<Project>));
            var result = output as OkNegotiatedContentResult<Project>;
            Assert.AreEqual(project.Id, result.Content.Id);
        }

        [TestMethod]
        public void ShouldGetNoProjectById()
        {
            var process = new Mock<IProjectManagementProcess>();
            var project = projects.FirstOrDefault(t => t.Id == 0);
            process.Setup(m => m.GetProjectByProjectId(It.IsAny<int>())).Returns(project);
            var controller = new ProjectController(process.Object);
            var output = controller.GetProjectById(0);
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(NotFoundResult));
        }
        [TestMethod]
        public void ShouldGetErrorOnGetProjectById()
        {
            var process = new Mock<IProjectManagementProcess>();
            process.Setup(m => m.GetProjectByProjectId(It.IsAny<int>())).Throws(new Exception("Server error"));
            var controller = new ProjectController(process.Object);
            var output = controller.GetProjectById(0);
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(ExceptionResult));
            var result = output as ExceptionResult;
            Assert.AreEqual("Server error", result.Exception.Message);
        }
        #endregion

        #region AddProject
        [TestMethod]
        public void ShouldCreateProject()
        {
            var projectId = new Random().Next(2, 100);
            var process = new Mock<IProjectManagementProcess>();
            process.Setup(m => m.AddProject(It.IsAny<Project>())).Returns<Project>(p =>
            {
                p.Id = projectId;
                return true;
            });
            var controller = new ProjectController(process.Object);
            controller.Request = new System.Net.Http.HttpRequestMessage
            {
                RequestUri = new Uri("http://localhost/api/createproject"),
                Method = System.Net.Http.HttpMethod.Post
            };
            var output = controller.CreateProject(new Project { });
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(CreatedNegotiatedContentResult<Project>));
            var result = output as CreatedNegotiatedContentResult<Project>;
            Assert.AreEqual(projectId, result.Content.Id);
        }

        [TestMethod]
        public void ShouldNotCreateProject()
        {
            var process = new Mock<IProjectManagementProcess>();
            process.Setup(m => m.AddProject(It.IsAny<Project>())).Returns(false);
            var controller = new ProjectController(process.Object);
            controller.Request = new System.Net.Http.HttpRequestMessage
            {
                RequestUri = new Uri("http://localhost/api/createproject"),
                Method = System.Net.Http.HttpMethod.Post
            };
            var output = controller.CreateProject(new Project { });
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public void ShouldGetErrorOnCreateProject()
        {
            var process = new Mock<IProjectManagementProcess>();
            process.Setup(m => m.AddProject(It.IsAny<Project>())).Throws(new Exception("Internal Error"));
            var controller = new ProjectController(process.Object);
            var output = controller.CreateProject(new Project { });
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(ExceptionResult));
            var result = output as ExceptionResult;
            Assert.AreEqual("Internal Error", result.Exception.Message);
        }
        #endregion

        #region UpdateProject
        [TestMethod]
        public void ShouldUpdateProject()
        {
            var process = new Mock<IProjectManagementProcess>();
            process.Setup(m => m.UpdateProject(It.IsAny<Project>())).Returns(true);
            var controller = new ProjectController(process.Object);
            var output = controller.UpdateProject(new Project { });
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(ResponseMessageResult));
            var result = output as ResponseMessageResult;
            Assert.IsNull(result.Response.Content);
            Assert.AreEqual(System.Net.HttpStatusCode.Accepted, result.Response.StatusCode);
        }
        [TestMethod]
        public void ShouldNotUpdateProject()
        {
            var process = new Mock<IProjectManagementProcess>();
            process.Setup(m => m.UpdateProject(It.IsAny<Project>())).Returns(false);
            var controller = new ProjectController(process.Object);
            var output = controller.UpdateProject(new Project { });
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(BadRequestErrorMessageResult));
        }
        [TestMethod]
        public void ShouldGetErrorOnUpdateProject()
        {
            var process = new Mock<IProjectManagementProcess>();
            process.Setup(m => m.UpdateProject(It.IsAny<Project>())).Throws(new Exception("Internal Error"));
            var controller = new ProjectController(process.Object);
            var output = controller.UpdateProject(new Project { });
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(ExceptionResult));
            var result = output as ExceptionResult;
            Assert.AreEqual("Internal Error", result.Exception.Message);
        }
        #endregion

        #region SuspendProject
        [TestMethod]
        public void ShouldSuspendProject()
        {
            var process = new Mock<IProjectManagementProcess>();
            process.Setup(m => m.SuspendProject(It.IsAny<int>())).Returns(true);
            var controller = new ProjectController(process.Object);
            var output = controller.SuspendProject(1);
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(ResponseMessageResult));
            var result = output as ResponseMessageResult;
            Assert.IsNull(result.Response.Content);
            Assert.AreEqual(System.Net.HttpStatusCode.Accepted, result.Response.StatusCode);
        }

        [TestMethod]
        public void ShouldNotSuspendProject()
        {
            var process = new Mock<IProjectManagementProcess>();
            process.Setup(m => m.SuspendProject(It.IsAny<int>())).Returns(false);
            var controller = new ProjectController(process.Object);
            var output = controller.SuspendProject(1);
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public void ShouldGetErrorOnSuspendProject()
        {
            var process = new Mock<IProjectManagementProcess>();
            process.Setup(m => m.SuspendProject(It.IsAny<int>())).Throws(new Exception("Internal Error"));
            var controller = new ProjectController(process.Object);
            var output = controller.SuspendProject(1);
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(ExceptionResult));
            var result = output as ExceptionResult;
            Assert.AreEqual("Internal Error", result.Exception.Message);
        }
        #endregion

        #region DeleteProject
        [TestMethod]
        public void ShouldDeleteProject()
        {
            var process = new Mock<IProjectManagementProcess>();
            process.Setup(m => m.DeleteProject(It.IsAny<int>())).Returns(true);
            var controller = new ProjectController(process.Object);
            var output = controller.DeleteProject(1);
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(ResponseMessageResult));
            var result = output as ResponseMessageResult;
            Assert.IsNull(result.Response.Content);
            Assert.AreEqual(System.Net.HttpStatusCode.Accepted, result.Response.StatusCode);
        }

        [TestMethod]
        public void ShouldNotDeleteProject()
        {
            var process = new Mock<IProjectManagementProcess>();
            process.Setup(m => m.DeleteProject(It.IsAny<int>())).Returns(false);
            var controller = new ProjectController(process.Object);
            var output = controller.DeleteProject(1);
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public void ShouldGetErrorOnDeleteProject()
        {
            var process = new Mock<IProjectManagementProcess>();
            process.Setup(m => m.DeleteProject(It.IsAny<int>())).Throws(new Exception("Internal Error"));
            var controller = new ProjectController(process.Object);
            var output = controller.DeleteProject(1);
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(ExceptionResult));
            var result = output as ExceptionResult;
            Assert.AreEqual("Internal Error", result.Exception.Message);
        }
        #endregion

        #endregion

        #region Tasks

        #region GetAllTasks
        [TestMethod]
        public void ShouldGetAllTasks()
        {
            var process = new Mock<ITaskManagementProcess>();
            process.Setup(m => m.GetTasks()).Returns(tasks);
            var controller = new TaskController(process.Object);
            var output = controller.GetAllTasks();
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(OkNegotiatedContentResult<IEnumerable<Task>>));
            var result = output as OkNegotiatedContentResult<IEnumerable<Task>>;
            Assert.AreEqual(tasks.Count(), result.Content.Count());
        }

        [TestMethod]
        public void ShouldGetNoTasks()
        {
            var process = new Mock<ITaskManagementProcess>();
            var taskInput = new Task[] { };
            process.Setup(m => m.GetTasks()).Returns(taskInput.AsQueryable());
            var controller = new TaskController(process.Object);
            var output = controller.GetAllTasks();
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(ResponseMessageResult));
            var result = output as ResponseMessageResult;
            Assert.IsNull(result.Response.Content);
            Assert.AreEqual(System.Net.HttpStatusCode.NoContent, result.Response.StatusCode);
        }
        [TestMethod]
        public void ShouldGetErrorOnGetAllTasks()
        {
            var process = new Mock<ITaskManagementProcess>();
            IQueryable<Task> taskInput = null;
            process.Setup(m => m.GetTasks()).Returns(taskInput);
            var controller = new TaskController(process.Object);
            var output = controller.GetAllTasks();
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(ExceptionResult));
            var result = output as ExceptionResult;
            Assert.AreEqual("Value cannot be null.\r\nParameter name: source", result.Exception.Message);
        }
        #endregion

        #region GetAllParentTasks
        [TestMethod]
        public void ShouldGetAllParentTasks()
        {
            var process = new Mock<ITaskManagementProcess>();
            process.Setup(m => m.GetParentTasks()).Returns(tasks.Where(t => t.IsParent));
            var controller = new TaskController(process.Object);
            var output = controller.GetAllParentTasks();
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(OkNegotiatedContentResult<IEnumerable<Task>>));
            var result = output as OkNegotiatedContentResult<IEnumerable<Task>>;
            Assert.AreEqual(tasks.Count(t => t.IsParent), result.Content.Count());
        }

        [TestMethod]
        public void ShouldGetNoParentTasks()
        {
            var process = new Mock<ITaskManagementProcess>();
            var taskInput = new Task[] { };
            process.Setup(m => m.GetParentTasks()).Returns(taskInput.AsQueryable());
            var controller = new TaskController(process.Object);
            var output = controller.GetAllParentTasks();
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(ResponseMessageResult));
            var result = output as ResponseMessageResult;
            Assert.IsNull(result.Response.Content);
            Assert.AreEqual(System.Net.HttpStatusCode.NoContent, result.Response.StatusCode);
        }
        [TestMethod]
        public void ShouldGetErrorOnGetAllParentTasks()
        {
            var process = new Mock<ITaskManagementProcess>();
            IQueryable<Task> taskInput = null;
            process.Setup(m => m.GetParentTasks()).Returns(taskInput);
            var controller = new TaskController(process.Object);
            var output = controller.GetAllParentTasks();
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(ExceptionResult));
            var result = output as ExceptionResult;
            Assert.AreEqual("Value cannot be null.\r\nParameter name: source", result.Exception.Message);
        }
        #endregion

        #region GetAllTasksByProjectId
        [TestMethod]
        public void ShouldGetAllTasksByProjectId()
        {
            var process = new Mock<ITaskManagementProcess>();
            process.Setup(m => m.GetTasksByProjectId(It.IsAny<int>())).Returns<int>(t => tasks.Where(x => x.Project.Id == t));
            var controller = new TaskController(process.Object);
            var output = controller.GetAllTasksByProjectId(1);
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(OkNegotiatedContentResult<IEnumerable<Task>>));
            var result = output as OkNegotiatedContentResult<IEnumerable<Task>>;
            Assert.AreEqual(tasks.Count(x => x.Project.Id == 1), result.Content.Count());
        }

        [TestMethod]
        public void ShouldGetNoTasksByProjectId()
        {
            var process = new Mock<ITaskManagementProcess>();
            var taskInput = new Task[] { };
            process.Setup(m => m.GetTasksByProjectId(It.IsAny<int>())).Returns(taskInput.AsQueryable());
            var controller = new TaskController(process.Object);
            var output = controller.GetAllTasksByProjectId(1);
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(ResponseMessageResult));
            var result = output as ResponseMessageResult;
            Assert.IsNull(result.Response.Content);
            Assert.AreEqual(System.Net.HttpStatusCode.NoContent, result.Response.StatusCode);
        }
        [TestMethod]
        public void ShouldGetErrorOnGetAllTasksByProjectId()
        {
            var process = new Mock<ITaskManagementProcess>();
            IQueryable<Task> taskInput = null;
            process.Setup(m => m.GetTasksByProjectId(It.IsAny<int>())).Returns(taskInput);
            var controller = new TaskController(process.Object);
            var output = controller.GetAllTasksByProjectId(1);
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(ExceptionResult));
            var result = output as ExceptionResult;
            Assert.AreEqual("Value cannot be null.\r\nParameter name: source", result.Exception.Message);
        }
        #endregion

        #region GetAllParentTasksByProjectId
        [TestMethod]
        public void ShouldGetAllParentTasksByProjectId()
        {
            var process = new Mock<ITaskManagementProcess>();
            process.Setup(m => m.GetParentTasksByProjectId(It.IsAny<int>())).Returns<int>(t => tasks.Where(x => x.Project.Id == t && x.IsParent));
            var controller = new TaskController(process.Object);
            var output = controller.GetAllParentTasksByProjectId(1);
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(OkNegotiatedContentResult<IEnumerable<Task>>));
            var result = output as OkNegotiatedContentResult<IEnumerable<Task>>;
            Assert.AreEqual(tasks.Count(x => x.Project.Id == 1 && x.IsParent), result.Content.Count());
        }

        [TestMethod]
        public void ShouldGetNoParentTasksByProjectId()
        {
            var process = new Mock<ITaskManagementProcess>();
            var taskInput = new Task[] { };
            process.Setup(m => m.GetParentTasksByProjectId(It.IsAny<int>())).Returns(taskInput.AsQueryable());
            var controller = new TaskController(process.Object);
            var output = controller.GetAllParentTasksByProjectId(1);
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(ResponseMessageResult));
            var result = output as ResponseMessageResult;
            Assert.IsNull(result.Response.Content);
            Assert.AreEqual(System.Net.HttpStatusCode.NoContent, result.Response.StatusCode);
        }
        [TestMethod]
        public void ShouldGetErrorOnGetAllParentTasksByProjectId()
        {
            var process = new Mock<ITaskManagementProcess>();
            IQueryable<Task> taskInput = null;
            process.Setup(m => m.GetParentTasksByProjectId(It.IsAny<int>())).Returns(taskInput);
            var controller = new TaskController(process.Object);
            var output = controller.GetAllParentTasksByProjectId(1);
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(ExceptionResult));
            var result = output as ExceptionResult;
            Assert.AreEqual("Value cannot be null.\r\nParameter name: source", result.Exception.Message);
        }
        #endregion

        #region GetTaskByTaskId
        [TestMethod]
        public void ShouldGetTaskByTaskId()
        {
            var process = new Mock<ITaskManagementProcess>();
            var task = tasks.FirstOrDefault(t => t.Id == 1);
            process.Setup(m => m.GetTaskByTaskId(1)).Returns(task);
            var controller = new TaskController(process.Object);
            var output = controller.GetTaskByTaskId(1);
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(OkNegotiatedContentResult<Task>));
            var result = output as OkNegotiatedContentResult<Task>;
            Assert.AreEqual(task.Id, result.Content.Id);
        }

        [TestMethod]
        public void ShouldGetNoTaskById()
        {
            var process = new Mock<ITaskManagementProcess>();
            var task = tasks.FirstOrDefault(t => t.Id == 0);
            process.Setup(m => m.GetTaskByTaskId(0)).Returns(task);
            var controller = new TaskController(process.Object);
            var output = controller.GetTaskByTaskId(0);
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(NotFoundResult));
        }
        [TestMethod]
        public void ShouldGetErrorOnGetTaskById()
        {
            var process = new Mock<ITaskManagementProcess>();
            process.Setup(m => m.GetTaskByTaskId(0)).Throws(new Exception("Server error"));
            var controller = new TaskController(process.Object);
            var output = controller.GetTaskByTaskId(0);
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(ExceptionResult));
            var result = output as ExceptionResult;
            Assert.AreEqual("Server error", result.Exception.Message);
        }
        #endregion

        #region AddTask
        [TestMethod]
        public void ShouldCreateTask()
        {
            var taskId = new Random().Next(2, 100);
            var process = new Mock<ITaskManagementProcess>();
            process.Setup(m => m.AddTask(It.IsAny<Task>())).Returns<Task>(t =>
            {
                t.Id = taskId;
                return true;
            });
            var controller = new TaskController(process.Object);
            controller.Request = new System.Net.Http.HttpRequestMessage
            {
                RequestUri = new Uri("http://localhost/api/createtask"),
                Method = System.Net.Http.HttpMethod.Post
            };
            var output = controller.CreateTask(new Task { });
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(CreatedNegotiatedContentResult<Task>));
            var result = output as CreatedNegotiatedContentResult<Task>;
            Assert.AreEqual(taskId, result.Content.Id);
        }

        [TestMethod]
        public void ShouldNotCreateTask()
        {
            var process = new Mock<ITaskManagementProcess>();
            process.Setup(m => m.AddTask(It.IsAny<Task>())).Returns(false);
            var controller = new TaskController(process.Object);
            controller.Request = new System.Net.Http.HttpRequestMessage
            {
                RequestUri = new Uri("http://localhost/api/createtask"),
                Method = System.Net.Http.HttpMethod.Post
            };
            var output = controller.CreateTask(new Task { });
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public void ShouldGetErrorOnCreateTask()
        {
            var process = new Mock<ITaskManagementProcess>();
            process.Setup(m => m.AddTask(It.IsAny<Task>())).Throws(new Exception("Internal Error"));
            var controller = new TaskController(process.Object);
            var output = controller.CreateTask(new Task { });
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(ExceptionResult));
            var result = output as ExceptionResult;
            Assert.AreEqual("Internal Error", result.Exception.Message);
        }
        #endregion

        #region UpdateTask
        [TestMethod]
        public void ShouldUpdateTask()
        {
            var process = new Mock<ITaskManagementProcess>();
            process.Setup(m => m.UpdateTask(It.IsAny<Task>())).Returns(true);
            var controller = new TaskController(process.Object);
            var output = controller.UpdateTask(new Task { });
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(ResponseMessageResult));
            var result = output as ResponseMessageResult;
            Assert.IsNull(result.Response.Content);
            Assert.AreEqual(System.Net.HttpStatusCode.Accepted, result.Response.StatusCode);
        }
        [TestMethod]
        public void ShouldNotUpdateTask()
        {
            var process = new Mock<ITaskManagementProcess>();
            process.Setup(m => m.UpdateTask(It.IsAny<Task>())).Returns(false);
            var controller = new TaskController(process.Object);
            var output = controller.UpdateTask(new Task { });
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(BadRequestErrorMessageResult));
        }
        [TestMethod]
        public void ShouldGetErrorOnUpdateTask()
        {
            var process = new Mock<ITaskManagementProcess>();
            process.Setup(m => m.UpdateTask(It.IsAny<Task>())).Throws(new Exception("Internal Error"));
            var controller = new TaskController(process.Object);
            var output = controller.UpdateTask(new Task { });
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(ExceptionResult));
            var result = output as ExceptionResult;
            Assert.AreEqual("Internal Error", result.Exception.Message);
        }
        #endregion

        #region CompleteTask
        [TestMethod]
        public void ShouldCompleteTask()
        {
            var process = new Mock<ITaskManagementProcess>();
            process.Setup(m => m.CompleteTask(It.IsAny<int>())).Returns(true);
            var controller = new TaskController(process.Object);
            var output = controller.CompleteTask(1);
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(ResponseMessageResult));
            var result = output as ResponseMessageResult;
            Assert.IsNull(result.Response.Content);
            Assert.AreEqual(System.Net.HttpStatusCode.Accepted, result.Response.StatusCode);
        }

        [TestMethod]
        public void ShouldNotCompleteTask()
        {
            var process = new Mock<ITaskManagementProcess>();
            process.Setup(m => m.CompleteTask(It.IsAny<int>())).Returns(false);
            var controller = new TaskController(process.Object);
            var output = controller.CompleteTask(1);
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public void ShouldGetErrorOnCompleteTask()
        {
            var process = new Mock<ITaskManagementProcess>();
            process.Setup(m => m.CompleteTask(It.IsAny<int>())).Throws(new Exception("Internal Error"));
            var controller = new TaskController(process.Object);
            var output = controller.CompleteTask(1);
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(ExceptionResult));
            var result = output as ExceptionResult;
            Assert.AreEqual("Internal Error", result.Exception.Message);
        }
        #endregion

        #region DeleteTask
        [TestMethod]
        public void ShouldDeleteTask()
        {
            var process = new Mock<ITaskManagementProcess>();
            process.Setup(m => m.DeleteTask(It.IsAny<int>())).Returns(true);
            var controller = new TaskController(process.Object);
            var output = controller.DeleteTask(1);
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(ResponseMessageResult));
            var result = output as ResponseMessageResult;
            Assert.IsNull(result.Response.Content);
            Assert.AreEqual(System.Net.HttpStatusCode.Accepted, result.Response.StatusCode);
        }

        [TestMethod]
        public void ShouldNotDeleteTask()
        {
            var process = new Mock<ITaskManagementProcess>();
            process.Setup(m => m.DeleteTask(It.IsAny<int>())).Returns(false);
            var controller = new TaskController(process.Object);
            var output = controller.DeleteTask(1);
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public void ShouldGetErrorOnDeleteTask()
        {
            var process = new Mock<ITaskManagementProcess>();
            process.Setup(m => m.DeleteTask(It.IsAny<int>())).Throws(new Exception("Internal Error"));
            var controller = new TaskController(process.Object);
            var output = controller.DeleteTask(1);
            Assert.IsNotNull(output);
            Assert.IsInstanceOfType(output, typeof(ExceptionResult));
            var result = output as ExceptionResult;
            Assert.AreEqual("Internal Error", result.Exception.Message);
        }
        #endregion

        #endregion
    }

}
