using Core.Models;
using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Services
{
    public class FolderService
    {
        public void Create(string name, Guid userId, Guid? parentId)
        {
            using var db = new AppDbContext();

            db.Folders.Add(new Folder
            {
                Id = Guid.NewGuid(),
                Name = name,
                UserId = userId,
                ParentFolderId = parentId
            });

            db.SaveChanges();
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
            if (string.IsNullOrWhiteSpace(newName))
                return;

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
