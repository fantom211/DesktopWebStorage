using Core.Models;
using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class UserService
    {
        public User? GetById(Guid id)
        {
            using var db = new AppDbContext();
            return db.Users.FirstOrDefault(u => u.Id == id);
        }

        public User? GetByUsername(string username)
        {
            using var db = new AppDbContext();
            return db.Users.FirstOrDefault(u => u.Username == username);
        }
    }
}
