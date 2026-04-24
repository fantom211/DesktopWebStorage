using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<FileEntity> Files { get; set; } = new List<FileEntity>();
        public ICollection<Folder> Folders { get; set; } = new List<Folder>();
    }
}
