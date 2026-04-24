using Core.Models;
using Data;
using Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Text;
using System.Threading.Tasks;


namespace Services
{
    public class FileService
    {
        private readonly FileStorage storage = new FileStorage();

        public void Upload(string path, User user, Guid? folderId)
        {
            using var db = new AppDbContext();

            var savedPath = storage.SaveFile(path, user);

            var fileName = Path.GetFileName(path);

            var file = new FileEntity
            {
                Name = fileName,
                Extension = Path.GetExtension(path),  
                PhysicalPath = savedPath,
                UserId = user.Id,
                FolderId = folderId,
                Size = new FileInfo(savedPath).Length
            };

            db.Files.Add(file);
            db.SaveChanges();
        }

        public void Delete(Guid fileId)
        {
            using var db = new AppDbContext();

            var file = db.Files.FirstOrDefault(f => f.Id == fileId);
            if (file == null) return;

            storage.DeleteFile(file.PhysicalPath);

            db.Files.Remove(file);
            db.SaveChanges();
        }

        public void Rename(Guid fileId, string newName)
        {
            using var db = new AppDbContext();

            var file = db.Files.FirstOrDefault(f => f.Id == fileId);
            if (file == null) return;

            var newPath = Path.Combine(Path.GetDirectoryName(file.PhysicalPath)!, newName);

            storage.MoveFile(file.PhysicalPath, newPath);

            file.PhysicalPath = newPath;
            file.Name = newName;

            db.SaveChanges();
        }
    }
}
