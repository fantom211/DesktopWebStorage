using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class FileEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = "";
        public string Extension { get; set; } = "";

        public long Size { get; set; }

        public string PhysicalPath { get; set; } = "";

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid? FolderId { get; set; }
        public Folder? Folder { get; set; }
    }
}
