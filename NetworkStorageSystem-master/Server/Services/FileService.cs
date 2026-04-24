using Core.Models;
using Data;
using Microsoft.Identity.Client.Extensions.Msal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace Server.Services
{
    public class FileService
    {
        private readonly FileStorage storage = new FileStorage();
        static string storageRoot = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "StorageRoot"
        );
        public void Upload(string username, Guid? folderId, string fileName, byte[] data)
        {
            using var db = new AppDbContext();

            var user = db.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return;

            string extension = Path.GetExtension(fileName);
            var nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);

            string userDir = Path.Combine(storageRoot, username);
            Directory.CreateDirectory(userDir);

            string fullPath = Path.Combine(userDir, fileName);

            File.WriteAllBytes(fullPath, data);

            db.Files.Add(new FileEntity
            {
                Id = Guid.NewGuid(),
                Name = nameWithoutExt,
                Extension = extension,
                Size = data.Length,
                PhysicalPath = fullPath,
                UserId = user.Id,
                FolderId = folderId
            });

            db.SaveChanges();
        }

        public List<FileEntity> GetUserFiles(Guid userId, Guid? folderId)
        {
            using var db = new AppDbContext();

            var query = db.Files.Where(f => f.UserId == userId);

            if (folderId == null)
                query = query.Where(f => f.FolderId == null);
            else
                query = query.Where(f => f.FolderId == folderId);

            return query.ToList();
        }

        public void Delete(Guid fileId)
        {
            using var db = new AppDbContext();

            var file = db.Files.FirstOrDefault(f => f.Id == fileId);
            if (file == null) return;

            storage.Delete(file.PhysicalPath);

            db.Files.Remove(file);
            db.SaveChanges();
        }

        public void Rename(Guid fileId, string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                return;

            using var db = new AppDbContext();

            var file = db.Files.FirstOrDefault(f => f.Id == fileId);
            if (file == null) return;

            var directory = Path.GetDirectoryName(file.PhysicalPath)!;
            var fullName = newName + file.Extension;

            var newPath = Path.Combine(directory, fullName);

            storage.MoveFile(file.PhysicalPath, newPath);

            file.Name = newName;
            file.PhysicalPath = newPath;

            db.SaveChanges();
        }
    }
}
