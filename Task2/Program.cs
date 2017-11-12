using System;
using System.IO;
using System.Security.Permissions;

namespace Task2
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: Watcher.exe (directory) (action)");
                return;
            }

            if (args[1].Equals("-w"))
                RunWatch(args[0]);
            else if (args[1].Equals("-b"))
                Backup(args[0]);
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static void Backup(string path)
        {
            string backupFolderPath = GetBackupFolderPath(path);
            string[] dirs = Directory.GetDirectories(backupFolderPath,
                "*", SearchOption.TopDirectoryOnly);

            Console.WriteLine("Доступные для восстановления:");
            foreach (string dir in dirs)
                Console.WriteLine(Path.GetFileName(dir));

            Console.Write("Введите дату отката: ");
            string selectBackup = Console.ReadLine();
            selectBackup = backupFolderPath + "\\" + selectBackup;
            if (Directory.Exists(selectBackup))
                CopyAllTextFiles(selectBackup, path);
            else
                Console.WriteLine("Invalid input backup name");
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static void RunWatch(string path)
        {
            FileSystemWatcher watcher = new FileSystemWatcher
            {
                IncludeSubdirectories = true,
                Path = path,
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                | NotifyFilters.FileName | NotifyFilters.DirectoryName,
                Filter = "*.txt"
            };

            Directory.CreateDirectory(GetBackupFolderPath(watcher.Path));

            // Add event handlers.
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnChanged);
            watcher.Renamed += new RenamedEventHandler(OnChanged);

            // Begin watching.
            watcher.EnableRaisingEvents = true;

            // Wait for the user to quit the program.
            Console.WriteLine("Press \'q\' to quit the watch mode.");
            while (Console.Read() != 'q') ;
        }
        
        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            FileSystemWatcher watcher = source as FileSystemWatcher;
            string destinationPath = GetBackupFolderPath(watcher.Path) + "\\"  + DateTime.Now.ToString("d_M_yyyy_hh_mm_ss");

            CopyAllTextFiles(watcher.Path, destinationPath);
        }

        private static void CopyAllTextFiles(string source, string destination)
        {
            foreach (string dir in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dir.Replace(source, destination));

            foreach (string path in Directory.GetFiles(source, "*.txt", SearchOption.AllDirectories))
                File.Copy(path, path.Replace(source, destination), true);
        }

        private static string GetBackupFolderPath(string path)
        {
            return path + "\\..\\backup_" + Path.GetFileName(path);
        }
    }
}
