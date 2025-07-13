using DbOperation.Interface;
using DbOperation.Models;
using iText.Layout.Tagging;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbOperation.Implementation
{
    public class UsersService : IUserService
    {
        private readonly DbContextOptions<Assignment4Context> _context;

        public UsersService(string context)
        {
            _context = new DbContextOptionsBuilder<Assignment4Context>().UseSqlServer(context).Options;
        }


        public bool AddUser(Users user)
        {
            try
            {
                using (var db = new Assignment4Context(_context))
                {
                    db.Users.Add(user);
                    var affectedRows = db.SaveChanges();
                    return affectedRows > 0; // True if a row was inserted
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }



        public List<Users> GetAllUsers()
        {
            using (var db = new Assignment4Context(_context))
            {
                var a = db.Users.ToList();
                return db.Users.ToList();
            }
        }
        public Users? ValidateUser(string UserName, string password)
        {
            try
            {
                using (var db = new Assignment4Context(_context))
                {
                    // Retrieve the user by username
                    var user = db.Users.FirstOrDefault(u => u.username == UserName);

                    if (user == null)
                    {
                        // User not found
                        return null;
                    }

                    // Check if the password matches (assumes password is stored as plain text, you should hash it in production)
                    if (user.passwordHash == password)
                    {
                        // Password matches, return the user
                        return user;
                    }

                    // Password does not match
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error validating user: {ex.Message}");
                return null;
            }
        }



        public bool UpdateUser(Users user)
        {
            using (var db = new Assignment4Context(_context))
            {
                db.Users.Update(user);
                return db.SaveChanges() > 0;
            }
        }

        public bool DeleteUser(int userId)
        {
            using (var db = new Assignment4Context(_context))
            {
                var user = db.Users.FirstOrDefault(u => u.userId == userId);
                if (user != null)
                {
                    db.Users.Remove(user);
                    return db.SaveChanges() > 0;
                }
                return false;
            }
        }
    }
}
