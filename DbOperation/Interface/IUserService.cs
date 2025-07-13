using DbOperation.Models;
using System.Collections.Generic;

namespace DbOperation.Interface
{
    public interface IUserService
    {
        bool AddUser(Users user);
        List<Users> GetAllUsers();
        bool UpdateUser(Users user);
        bool DeleteUser(int userId);

        Users? ValidateUser(string UserName, string password); // Add this line
    }

}
