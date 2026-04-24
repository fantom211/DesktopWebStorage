using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class FileStorage
    {
        private readonly string root = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "StorageRoot");

        public FileStorage()
        {
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);
        }

        public string Save(string fileName, string username, byte[] data)
        {
            var userDir = Path.Combine(root, username);

            if (!Directory.Exists(userDir))
                Directory.CreateDirectory(userDir);

            var path = Path.Combine(userDir, fileName);

            File.WriteAllBytes(path, data);

            return path;
        }

        public void Delete(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }

        public void MoveFile(string oldPath, string newPath)
        {
            if (!File.Exists(oldPath))
                return;

            var directory = Path.GetDirectoryName(newPath);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory!);

            // если файл уже существует — удаляем (или можно сделать return)
            if (File.Exists(newPath))
                File.Delete(newPath);

            File.Move(oldPath, newPath);
        }
    }
}
