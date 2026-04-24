using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Folder
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = null!;

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid? ParentFolderId { get; set; }
        public Folder? ParentFolder { get; set; }

        public ICollection<Folder> Children { get; set; } = new List<Folder>();
        public ICollection<FileEntity> Files { get; set; } = new List<FileEntity>();
    }
}
