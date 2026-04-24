using Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class NetworkClient
    {
        private readonly string host = "127.0.0.1";
        private readonly int port = 8888;

        private string Send(string message)
        {
            using var client = new TcpClient(host, port);
            using var stream = client.GetStream();

            var data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);

            var buffer = new byte[8192];
            int read = stream.Read(buffer, 0, buffer.Length);

            return Encoding.UTF8.GetString(buffer, 0, read);
        }

        // ---------------- AUTH ----------------

        public User? Login(string username, string password)
        {
            var response = Send($"LOGIN|{username}|{password}");

            if (!response.StartsWith("OK"))
                return null;

            var id = response.Split('|')[1];

            return new User
            {
                Id = Guid.Parse(id),
                Username = username
            };
        }

        public bool Register(string username, string password)
        {
            var response = Send($"REGISTER|{username}|{password}");
            return response == "OK";
        }

        // ---------------- FILES ----------------

        public void UploadFile(string filePath, string username, Guid? folderId)
        {
            var fileName = Path.GetFileName(filePath);
            var data = File.ReadAllBytes(filePath);

            using var client = new TcpClient(host, port);
            using var stream = client.GetStream();

            var header =
                $"UPLOAD|{username}|{folderId?.ToString() ?? "null"}|{fileName}|{data.Length}\n";

            var headerBytes = Encoding.UTF8.GetBytes(header);
            stream.Write(headerBytes, 0, headerBytes.Length);
            stream.Flush();

            stream.Write(data, 0, data.Length);
            stream.Flush();

            // ВАЖНО: дождаться ответа
            var response = ReadResponse(stream);
            Console.WriteLine(response);
        }

        public List<FileEntity> GetFiles(Guid userId, Guid? folderId)
        {
            var response = Send($"LIST_FILES|{userId}|{folderId?.ToString() ?? "null"}");

            if (string.IsNullOrEmpty(response))
                return new List<FileEntity>();

            return response.Split(';')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x =>
                {
                    var p = x.Split(',');

                    return new FileEntity
                    {
                        Id = Guid.Parse(p[0]),
                        Name = p[1],
                        Extension = p[2],
                        Size = int.Parse(p[3])
                    };
                })
                .ToList();
        }

        public void DeleteFile(Guid fileId)
        {
            Send($"DELETE_FILE|{fileId}");
        }

        public void RenameFile(Guid fileId, string newName)
        {
            Send($"RENAME_FILE|{fileId}|{newName}");
        }

        // ---------------- FOLDERS ----------------

        public List<Folder> GetFolders(Guid userId)
        {
            var response = Send($"LIST_FOLDERS|{userId}");

            if (string.IsNullOrEmpty(response))
                return new List<Folder>();

            return response.Split(';')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x =>
                {
                    var p = x.Split(',');

                    return new Folder
                    {
                        Id = Guid.Parse(p[0]),
                        Name = p[1],
                        ParentFolderId = p[2] == "null"
                            ? null
                            : Guid.Parse(p[2])
                    };
                })
                .ToList();
        }

        public void CreateFolder(Guid userId, Guid? parentId, string name)
        {
            Send($"CREATE_FOLDER|{userId}|{parentId?.ToString() ?? "null"}|{name}");
        }

        public void RenameFolder(Guid folderId, string newName)
        {
            Send($"RENAME_FOLDER|{folderId}|{newName}");
        }

        public void DeleteFolder(Guid folderId)
        {
            Send($"DELETE_FOLDER|{folderId}");
        }

        // ---------------- helpers ----------------

        private string ReadResponse(NetworkStream stream)
        {

            try
            {
                var buffer = new byte[8192];
                int read = stream.Read(buffer, 0, buffer.Length);

                if (read == 0)
                    return "EMPTY";

                return Encoding.UTF8.GetString(buffer, 0, read);
            }
            catch (IOException ex)
            {
                return "CONNECTION_ERROR: " + ex.Message;
            }
        }
    }
}
