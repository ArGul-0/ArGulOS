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
        private static string OSVersion = "Alpha 0.1";
        private static string Separator25 = "=========================";
        private static string Separator50 = "==================================================";

        Sys.FileSystem.CosmosVFS fs;
        public static string CurrentDirectory = @"0:\";
        protected override void BeforeRun()
        {
            //Перед запуском
            Console.Clear();
            fs = new Sys.FileSystem.CosmosVFS();
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs);
            Console.WriteLine($"Welcome to {OSName}. Version: {OSVersion}\nEnter the \"help\" command to see the list of commands.\n");
        }

        protected override void Run()
        {
            //После запуска
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"{OSName} >>: ");
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
                    CurrentDirectory = Console.ReadLine();
                    break;
                case "ls":
                    try
                    {
                        var directory_list = Sys.FileSystem.VFS.VFSManager.GetDirectoryListing(CurrentDirectory);
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
                                Console.WriteLine("Error: Directory not found");
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
                    Console.WriteLine(CurrentDirectory);
                    break;
                case "mkfile":
                    Console.Write("Enter file name: ");
                    fileName = Console.ReadLine();
                    File.Create(CurrentDirectory + fileName);
                    break;
                case "mkdir":
                    Console.Write("Enter directory name: ");
                    dirName = Console.ReadLine();
                    Directory.CreateDirectory(CurrentDirectory + dirName);
                    break;
                case "rmfile":
                    try
                    {
                        Console.Write("Enter file name: ");
                        fileName = Console.ReadLine();
                        File.Delete(CurrentDirectory + fileName);
                        break;
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("Error: File not found");
                        break;
                    }
                case "rmdir":
                    try
                    {
                        Console.Write("Enter directory name: ");
                        fileName = Console.ReadLine();
                        Directory.Delete(CurrentDirectory + fileName);
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
            Console.WriteLine("Root folder: 0:\\\n");
            Console.WriteLine("This all commands:\n");
            Console.WriteLine("clear: clear all terminal");
            Console.WriteLine("shutdown: turning off the PC");
            Console.WriteLine("reboot: rebooting the PC");
            Console.WriteLine("sysinfo: information about the system");
            Console.WriteLine("ls: show all file and directorty in current directory");
            Console.WriteLine("cd: move to directory");
            Console.WriteLine("pwd: current directory");
            Console.WriteLine("mkfile: make file in current directory");
            Console.WriteLine("mkdir: make directory in current directory");
            Console.WriteLine("rmfile: remove file in current directory");
            Console.WriteLine("rmdir: remove directory in current directory");

            Console.WriteLine();
        }
    }
}
