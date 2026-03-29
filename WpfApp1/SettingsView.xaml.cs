using Microsoft.Win32;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Management;
using System.IO;

namespace WpfApp1
{
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();

            OsText.Text = "OS: " + GetWindowsVersion();
            CpuText.Text = "CPU: " + GetCPU();
            GpuText.Text = "GPU: " + GetGPU();
            RamText.Text = "RAM: " + GetRAM();

           
            var rustPath = FindRustPath();
            RustPathText.Text = rustPath != null
                ? "Rust: " + rustPath
                : "Rust: не найден";
        }

       
        private string? GetSteamPath()
        {
            var key = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam");
            return key?.GetValue("SteamPath") as string;
        }

       
        private string? FindRustPath()
        {
            string? steamPath = GetSteamPath();
            if (steamPath == null) return null;

            string libraryFile = Path.Combine(steamPath, @"steamapps\libraryfolders.vdf");
            if (!File.Exists(libraryFile)) return null;

            foreach (var line in File.ReadAllLines(libraryFile))
            {
                if (line.Contains("path"))
                {
                    string path = line.Split('"')[3].Replace(@"\\", @"\");
                    string rustPath = Path.Combine(path, @"steamapps\common\Rust");

                    if (Directory.Exists(rustPath))
                        return rustPath;
                }
            }

            return null;
        }

        private string GetWindowsVersion()
        {
            using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
            {
                return $"{key.GetValue("ProductName")} (Build {key.GetValue("CurrentBuild")})";
            }
        }

        private string GetCPU()
        {
            using (var searcher = new ManagementObjectSearcher("select Name from Win32_Processor"))
            {
                foreach (var item in searcher.Get())
                {
                    return item["Name"].ToString();
                }
            }
            return "Unknown CPU";
        }

        private string GetGPU()
        {
            var result = new StringBuilder();

            using (var searcher = new ManagementObjectSearcher("select Name from Win32_VideoController"))
            {
                foreach (var item in searcher.Get())
                {
                    string name = item["Name"]?.ToString() ?? "Unknown";

                    string type;

                    if (name.ToLower().Contains("intel") ||
                        name.ToLower().Contains("radeon graphics"))
                        type = "Integrated";
                    else
                        type = "Discrete";

                    result.AppendLine($"{name} ({type})");
                }
            }

            return result.Length > 0 ? result.ToString() : "Unknown GPU";
        }

        private string GetRAM()
        {
            ulong totalMemory = 0;

            foreach (var item in new ManagementObjectSearcher("SELECT Capacity FROM Win32_PhysicalMemory").Get())
            {
                totalMemory += (ulong)item["Capacity"];
            }

            double ram = totalMemory / (1024.0 * 1024 * 1024);

            return $"{Math.Round(ram, 1)} GB";
        }
    }
}