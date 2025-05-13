using Cosmos.System.FileSystem.VFS;
using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sys = Cosmos.System;

namespace ArGulOS
{
    public class Kernel : Sys.Kernel
    {
        private static string OSName = "ArGulOS";
        private static string OSVersion = "Beta 1.0";
        private static string Separator25 = "=========================";
        private static string Separator50 = "==================================================";

        Sys.FileSystem.CosmosVFS fs;
        public static string StartDirectory = @"0:\";
        public static string FullCurrentDirectory = StartDirectory;
        public static string CdDirectory;
        public static bool CdChanged = false;

        private bool hasAnyChar = false;
        protected override void BeforeRun()
        {
            fs = new Sys.FileSystem.CosmosVFS();
            VFSManager.RegisterVFS(fs);

            // 3) Получаем список всех дисков, которые видит VFS (обычно это ваш "0:")
            var disks = VFSManager.GetDisks();
            if (disks.Count > 0)
            {
                var disk = disks[0];

                // 4) Если на диске нет разделов (Partitions.Count == 0), создаём один
                if (disk.Partitions.Count == 0)
                {
                    // В этом примере создаём раздел с максимальным размером (MB)
                    var sizeMb = (int)(disk.Size / (512 * 512));
                    disk.CreatePartition(sizeMb);
                }

                // 5) Форматируем первый раздел в FAT32 (quick = true для быстрой заливки)
                disk.FormatPartition(0, "FAT32", quick: true);

                // 6) «Примонтируем» раздел, чтобы он появился как корневая «0:\»
                disk.MountPartition(0);
            }
            Console.Clear();

            Console.WriteLine($"Welcome to {OSName}. Version: {OSVersion}\nEnter the \"help\" command to see the list of commands.\n");
        }

        protected override void Run()
        {
            //После запуска
            Console.ForegroundColor = ConsoleColor.Red;
            if (CdDirectory != null && CdChanged == true)
            {
                FullCurrentDirectory = Path.Combine(FullCurrentDirectory, CdDirectory);
                CdChanged = false;
            }
            FullCurrentDirectory = Path.Combine(FullCurrentDirectory);
            Console.Write($"{OSName}: {FullCurrentDirectory}>>: ");
            Console.ForegroundColor = ConsoleColor.White;
            Commands();
        }

        public void Commands()
        {
            string fileName;
            string dirName;
            var input = Console.ReadLine();
            switch (input)
            {
                case "help":
                    PrintAllCommands();
                    break;
                case "clear":
                    Console.Clear();
                    break;
                case "shutdown":
                    Cosmos.System.Power.Shutdown();
                    break;
                case "reboot":
                    Cosmos.System.Power.Reboot();
                    break;
                case "sysinfo":
                    //Получаем данные о железе (Не всё, но часть)
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    string CPUBrand = Cosmos.Core.CPU.GetCPUBrandString();
                    string CPUVendor = Cosmos.Core.CPU.GetCPUVendorName();
                    uint AmoutOfRAM = Cosmos.Core.CPU.GetAmountOfRAM();
                    ulong AvailableRAM = Cosmos.Core.GCImplementation.GetAvailableRAM();
                    uint UsedRAM = Cosmos.Core.GCImplementation.GetUsedRAM();

                    Console.WriteLine(Separator50);
                    Console.WriteLine("CPU: {0}\nCPU Vendor: {1}\nAmount of RAM: {2} MB\nAvailable RAM: {3} MB\nUsed RAM: {4} Byte\n", CPUBrand, CPUVendor, AmoutOfRAM, AvailableRAM, UsedRAM);
                    Console.WriteLine(Separator50);
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case "cd":
                    Console.WriteLine("Enter directory name: ");
                    CdDirectory = Console.ReadLine();
                    hasAnyChar = !string.IsNullOrEmpty(CdDirectory) && CdDirectory.Trim().Length > 0;
                    if (hasAnyChar == false)
                    {
                        Console.WriteLine("Error: Invalid name!");
                        break;
                    }
                    if (Directory.Exists(FullCurrentDirectory + "\\" + CdDirectory) == false)
                    {
                        Console.WriteLine("Error: Directory not found");
                        CdDirectory = "";
                        break;
                    }
                    FullCurrentDirectory = Path.Combine(FullCurrentDirectory, CdDirectory);
                    break;
                case "cd .":
                    FullCurrentDirectory = StartDirectory;
                    break;
                case "ls":
                    try
                    {
                        var directory_list = Sys.FileSystem.VFS.VFSManager.GetDirectoryListing(FullCurrentDirectory);
                        if (directory_list == null || directory_list.Count == 0)
                        {
                            Console.WriteLine("No files or directories found in this directory.");
                            break;
                        }
                        foreach (var directoryEntry in directory_list)
                        {
                            try
                            {
                                var entryType = directoryEntry.mEntryType;
                                if (entryType == Sys.FileSystem.Listing.DirectoryEntryTypeEnum.File)
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine("| <File>           " + directoryEntry.mName);
                                    Console.ForegroundColor = ConsoleColor.White;
                                }
                                if (entryType == Sys.FileSystem.Listing.DirectoryEntryTypeEnum.Directory)
                                {
                                    Console.ForegroundColor = ConsoleColor.Blue;
                                    Console.WriteLine("| <Directory>      " + directoryEntry.mName);
                                    Console.ForegroundColor = ConsoleColor.White;
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Error: Directories or files not found");
                                Console.WriteLine(e.ToString());
                            }
                        }
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        break;
                    }
                case "pwd":
                    Console.WriteLine(FullCurrentDirectory);
                    break;
                case "mkfile":
                    Console.Write("Enter file name: ");
                    fileName = Console.ReadLine();
                    hasAnyChar = !string.IsNullOrEmpty(fileName) && fileName.Trim().Length > 0;
                    if (hasAnyChar == false)
                    {
                        Console.WriteLine("Error: Invalid name!");
                        break;
                    }
                    File.Create(FullCurrentDirectory + "\\" + fileName).Dispose();
                    break;
                case "mkdir":
                    Console.Write("Enter directory name: ");
                    dirName = Console.ReadLine();
                    hasAnyChar = !string.IsNullOrEmpty(dirName) && dirName.Trim().Length > 0;
                    if (hasAnyChar == false)
                    {
                        Console.WriteLine("Error: Invalid name!");
                        break;
                    }
                    Directory.CreateDirectory(FullCurrentDirectory + "\\" + dirName);
                    break;
                case "rmfile":
                    try
                    {
                        Console.Write("Enter file name: ");
                        fileName = Console.ReadLine();
                        File.Delete(FullCurrentDirectory + "\\" + fileName);
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: File not found");
                        break;
                    }
                case "rmdir":
                    try
                    {
                        Console.Write("Enter directory name: ");
                        dirName = Console.ReadLine();
                        Directory.Delete(FullCurrentDirectory + "\\" + dirName, recursive: true);
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: Directory not found");
                        break;
                    }

                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{input}: Command is not found. Enter the \"help\" command to see the list of commands.");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
        }
        private void PrintAllCommands()
        {
            Console.WriteLine();

            Console.WriteLine("Root folder: 0:\\\n");
            Console.WriteLine("This all commands:\n");
            Console.WriteLine("clear: clear all terminal");
            Console.WriteLine("shutdown: turning off the PC");
            Console.WriteLine("reboot: rebooting the PC");
            Console.WriteLine("sysinfo: information about the system");
            Console.WriteLine("ls: show all file and directorty in current directory");
            Console.WriteLine("cd: move to directory");
            Console.WriteLine("cd .: move to start directory");
            Console.WriteLine("pwd: current directory");
            Console.WriteLine("mkfile: make file in current directory");
            Console.WriteLine("mkdir: make directory in current directory");
            Console.WriteLine("rmfile: remove file in current directory");
            Console.WriteLine("rmdir: remove directory in current directory");

            Console.WriteLine();
        }
    }
}
