using Data;
using Server.Services;
using System.Net;
using System.Net.Sockets;
using System.Text;

internal class Program
{
    static string storageRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "StorageRoot");

    private static void Main(string[] args)
    {
        if (!Directory.Exists(storageRoot))
            Directory.CreateDirectory(storageRoot);

        TcpListener server = new TcpListener(IPAddress.Any, 8888);
        server.Start();

        Console.WriteLine("Server started on port 8888");

        while (true)
        {
            var client = server.AcceptTcpClient();
            Task.Run(() => HandleClient(client));
        }
    }

    static void HandleClient(TcpClient client)
    {
        using var stream = client.GetStream();

        try
        {
            byte[] buffer = new byte[8192];
            int read = stream.Read(buffer, 0, buffer.Length);

            string request = Encoding.UTF8.GetString(buffer, 0, read);

            Console.WriteLine("RAW REQUEST:");
            Console.WriteLine(request);

            string[] parts = request.Split('|');

            string command = parts[0];

            switch (command)
            {
                case "LOGIN":
                    HandleLogin(parts, stream);
                    break;

                case "REGISTER":
                    HandleRegister(parts, stream);
                    break;

                case "UPLOAD":
                    HandleUpload(parts, stream);
                    break;

                case "LIST_FILES":
                    HandleListFiles(parts, stream);
                    break;

                case "DELETE_FILE":
                    HandleDeleteFile(parts, stream);
                    break;

                case "RENAME_FILE":
                    HandleRenameFile(parts, stream);
                    break;

                case "CREATE_FOLDER":
                    HandleCreateFolder(parts, stream);
                    break;

                case "LIST_FOLDERS":
                    HandleListFolders(parts, stream);
                    break;

                case "RENAME_FOLDER":
                    HandleRenameFolder(parts, stream);
                    break;

                case "DELETE_FOLDER":
                    HandleDeleteFolder(parts, stream);
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            client.Close();
        }
    }

    static void HandleRegister(string[] parts, NetworkStream stream)
    {
        var service = new AuthService();

        bool success = service.Register(parts[1], parts[2]);

        Write(stream, success ? "OK" : "FAIL");
    }

    static void HandleLogin(string[] parts, NetworkStream stream)
    {
        var service = new AuthService();

        var user = service.Login(parts[1], parts[2]);

        if (user == null)
            Write(stream, "FAIL");
        else
            Write(stream, $"OK|{user.Id}");
    }

    static void HandleUpload(string[] parts, NetworkStream stream)
    {
        string username = parts[1];
        Guid? folderId = parts[2] == "null" ? null : Guid.Parse(parts[2]);

        string fileName = Path.GetFileName(parts[3]);
        int size = int.Parse(parts[4]);

        byte[] buffer = new byte[8192];
        byte[] data = new byte[size];
        int total = 0;

        while (total < size)
        {
            int read = stream.Read(buffer, 0, Math.Min(buffer.Length, size - total));
            if (read == 0) break;

            Buffer.BlockCopy(buffer, 0, data, total, read);
            total += read;
        }

        if (total != size)
        {
            Write(stream, "FAIL");
            return;
        }

        var service = new FileService();
        service.Upload(username, folderId, fileName, data);

        Write(stream, "OK");
    }

    static void HandleListFiles(string[] parts, NetworkStream stream)
    {
        var userId = Guid.Parse(parts[1]);
        Guid? folderId = parts[2] == "null" ? null : Guid.Parse(parts[2]);

        var service = new FileService();

        var files = service.GetUserFiles(userId, folderId);

        var result = string.Join(";", files.Select(f =>
            $"{f.Id},{f.Name},{f.Extension},{f.Size}"
        ));

        Write(stream, result);
        Console.WriteLine($"LIST_FILES for user={userId}, folder={folderId}");
        Console.WriteLine("FILES:");
        Console.WriteLine(result);
    }

    static void HandleDeleteFile(string[] parts, NetworkStream stream)
    {
        var fileId = Guid.Parse(parts[1]);

        var service = new FileService();
        service.Delete(fileId);

        Write(stream, "OK");
    }

    static void HandleRenameFile(string[] parts, NetworkStream stream)
    {
        var fileId = Guid.Parse(parts[1]);
        var newName = parts[2];

        var service = new FileService();
        service.Rename(fileId, newName);

        Write(stream, "OK");
    }

    static void HandleCreateFolder(string[] parts, NetworkStream stream)
    {
        var userId = Guid.Parse(parts[1]);
        Guid? parentId = parts[2] == "null" ? null : Guid.Parse(parts[2]);
        var name = parts[3];

        var service = new FolderService();
        service.Create(name, userId, parentId);

        Write(stream, "OK");
    }

    static void HandleListFolders(string[] parts, NetworkStream stream)
    {
        var userId = Guid.Parse(parts[1]);

        var service = new FolderService();
        var folders = service.GetUserFolders(userId);

        var result = string.Join(";", folders.Select(f =>
            $"{f.Id},{f.Name},{f.ParentFolderId}"
        ));

        Write(stream, result);
    }

    static void HandleRenameFolder(string[] parts, NetworkStream stream)
    {
        var folderId = Guid.Parse(parts[1]);
        var newName = parts[2];

        var service = new FolderService();
        service.Rename(folderId, newName);

        Write(stream, "OK");
    }

    static void HandleDeleteFolder(string[] parts, NetworkStream stream)
    {
        var folderId = Guid.Parse(parts[1]);

        using var db = new AppDbContext();

        var folder = db.Folders.FirstOrDefault(f => f.Id == folderId);
        if (folder != null)
        {
            db.Folders.Remove(folder);
            db.SaveChanges();
        }

        Write(stream, "OK");
    }

    static void Write(NetworkStream stream, string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        stream.Write(data, 0, data.Length);
    }
}