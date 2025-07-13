using DbOperation.Interface;
using DbOperation.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace EventManagement.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        public new IActionResult User()
        {
            return View();
        }
        public new IActionResult LoginPage()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddOrUpdateUser(Users user)
        {
            if (user.userId != 0)
            {
                
                var result = _userService.UpdateUser(user);
                if (!result)
                {
                    return Json(new { success = false, message = "Failed to update user." });
                }
                return Json(new { success = true, message = "User updated successfully." });
            }
            else
            {
                var result = _userService.AddUser(user);
                if (!result)
                {
                    return Json(new { success = false, message = "Failed to add user." });
                }
                return Json(new { success = true, message = "User added successfully." });
            }
        }

        public IActionResult Delete(int id)
        {
            var result = _userService.DeleteUser(id);
            if (!result)
            {
                return Json(new { success = false, message = "Failed to delete user." });
            }
            return Json(new { success = true, message = "User deleted successfully." });
        }

        public IActionResult GetAllUsers()
        {
            var result = _userService.GetAllUsers();
            if (result == null || result.Count == 0)
            {
                return Json(new { success = false, message = "No users found." });
            }
            return Json(result);
        }
        public IActionResult GetUserByUserName(string username, string password)
        {
            var result = _userService.ValidateUser(username, password);
            if (result == null)
            {
                return Json(new { success = false, message = "No user found." });
            }
            else
            {
                return Json(new { success = true, message = "User found. Login successful.", user = result });
            }



        }
    }
}
