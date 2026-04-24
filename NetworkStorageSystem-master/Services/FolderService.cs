using Core.Models;
using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class FolderService
    {
        public Folder CreateFolder(string name, Guid userId, Guid? parentId = null)
        {
            using var db = new AppDbContext();

            var folder = new Folder
            {
                Name = name,
                UserId = userId,
                ParentFolderId = parentId
            };

            db.Folders.Add(folder);
            db.SaveChanges();

            return folder;
        }

        public List<Folder> GetUserFolders(Guid userId)
        {
            using var db = new AppDbContext();

            return db.Folders
                .Where(f => f.UserId == userId)
                .ToList();
        }

        public void Rename(Guid folderId, string newName)
        {
            using var db = new AppDbContext();

            var folder = db.Folders.FirstOrDefault(f => f.Id == folderId);
            if (folder == null) return;

            folder.Name = newName;
            db.SaveChanges();
        }

        public void Delete(Guid folderId)
        {
            using var db = new AppDbContext();

            var folder = db.Folders.FirstOrDefault(f => f.Id == folderId);
            if (folder == null) return;

            db.Folders.Remove(folder);
            db.SaveChanges();
        }
    }
}
