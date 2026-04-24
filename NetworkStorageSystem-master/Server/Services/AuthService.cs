using Core.Models;
using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Services
{
    public class AuthService
    {
        public User? Login(string username, string password)
        {
            using var db = new AppDbContext();

            return db.Users
                .FirstOrDefault(u => u.Username == username && u.Password == password);
        }

        public bool Register(string username, string password)
        {
            using var db = new AppDbContext();

            if (db.Users.Any(u => u.Username == username))
                return false;

            db.Users.Add(new User
            {
                Id = Guid.NewGuid(),
                Username = username,
                Password = password
            });

            db.SaveChanges();
            return true;
        }
    }
}