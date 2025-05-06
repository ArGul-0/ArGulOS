using System;
using System.Collections.Generic;
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
        protected override void BeforeRun()
        {
            //Перед запуском
            Console.Clear();
            Console.WriteLine($"Welcome to {OSName}. Version: {OSVersion}\nEnter the \"help\" command to see the list of commands.\n");
        }

        protected override void Run()
        {
            //После запуска
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"{OSName} >>: ");
            Console.ForegroundColor = ConsoleColor.White;
            var input = Console.ReadLine();
            switch ( input )
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

                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{input}: Command is not found. Enter the \"help\" command to see the list of commands.");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
        }
        private void PrintAllCommands()
        {
            Console.WriteLine("This all commands:\n");
            Console.WriteLine("clear: clear all terminal");
            Console.WriteLine("shutdown: turning off the PC");
            Console.WriteLine("reboot: rebooting the PC");
            Console.WriteLine("sysinfo: information about the system");

            Console.WriteLine();
        }
    }
}
