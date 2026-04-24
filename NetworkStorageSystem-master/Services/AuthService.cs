using Core.Models;
using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class AuthService
    {
        public bool Register(string username, string password)
        {
            using var db = new AppDbContext();

            var exists = db.Users.Any(u => u.Username == username);
            if (exists) return false;

            var user = new User
            {
                Username = username,
                Password = password
            };

            db.Users.Add(user);
            db.SaveChanges();

            return true;
        }

        public User? Login(string username, string password)
        {
            using var db = new AppDbContext();

            var user = db.Users.FirstOrDefault(u => u.Username == username);

            if (user == null)
                return null;

            if (user.Password != password)
                return null;

            return user;
        }
    }
}
