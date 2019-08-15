using ProjectManagement.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.BusinessLayer
{
    public interface IUserManagementProcess
    {
        IEnumerable<User> GetUsers();
        User GetUserByUserId(int id);
        bool AddUser(User user);
        bool UpdateUser(User user);
        bool DeleteUser(int id);
    }
}
