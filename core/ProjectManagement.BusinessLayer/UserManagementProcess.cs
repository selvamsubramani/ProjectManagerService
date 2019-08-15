using ProjectManagement.DataLayer;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ProjectManagement.BusinessLayer
{
    [ExcludeFromCodeCoverage]
    public class UserManagementProcess : IUserManagementProcess
    {
        private readonly IProjectManagementDataConnector _connector;
        public UserManagementProcess() : this(ProjectManagementDataConnector.Instance) { }
        public UserManagementProcess(IProjectManagementDataConnector connector)
        {
            _connector = connector;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool AddUser(Entities.User user)
        {
            if (!_connector.GetAllUsers().Any(u => u.FirstName == user.FirstName && u.LastName == user.LastName && u.EmployeeID == user.EmployeeId))
            {
                _connector.AddUser(ConvertToDataUser(user));
                return true;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteUser(int id)
        {
            if (_connector.GetUserById(id) != null)
            {
                _connector.DeleteUser(id);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Entities.User GetUserByUserId(int id)
        {
            var user = _connector.GetUserById(id);
            if (user != null)
                return ConvertToEntityUser(user);
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Entities.User> GetUsers()
        {
            return _connector.GetAllUsers().ToArray().Select(user => ConvertToEntityUser(user));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool UpdateUser(Entities.User user)
        {
            if (_connector.GetUserById(user.Id) != null)
            {
                _connector.UpdateUser(ConvertToDataUser(user));
                return true;
            }
            return false;
        }

        public Entities.User ConvertToEntityUser(User user)
        {
            return
                new Entities.User
                {
                    Id = user.ID,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    EmployeeId = user.EmployeeID
                };
        }

        public User ConvertToDataUser(Entities.User user)
        {
            return
                new User
                {
                    ID = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    EmployeeID = user.EmployeeId
                };
        }
    }
}
