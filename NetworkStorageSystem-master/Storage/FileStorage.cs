using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storage
{
    public class FileStorage
    {
        private readonly string root = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "StorageRoot");

        public FileStorage()
        {
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);
        }

        public string SaveFile(string sourcePath, User user)
        {
            var userDir = Path.Combine(root, user.Id.ToString());

            if (!Directory.Exists(userDir))
                Directory.CreateDirectory(userDir);

            var fileName = Path.GetFileName(sourcePath);
            var destPath = Path.Combine(userDir, fileName);

            File.Copy(sourcePath, destPath, true);

            return destPath;
        }

        public void DeleteFile(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }

        public void MoveFile(string oldPath, string newPath)
        {
            if (File.Exists(oldPath))
                File.Move(oldPath, newPath, true);
        }
    }
}
