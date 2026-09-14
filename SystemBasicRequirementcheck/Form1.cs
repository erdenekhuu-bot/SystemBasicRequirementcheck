using System.Runtime.InteropServices;

namespace SystemBasicRequirementcheck
{
    public partial class Form1 : Form
    {
        [StructLayout(LayoutKind.Sequential)]
        private class MEMORYSTATUSEX
        {
            public uint dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);
        public Form1()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Text = "Windows system detector";
            initList();
        }

       
        private void initList(){
            listView.View = View.Details;
            listView.FullRowSelect = true;
            listView.GridLines = true;
            listView.MultiSelect = false;
            listView.HideSelection = false;
            listView.Scrollable = false;

            listView.Columns.Clear();
            listView.Columns.Add("Subsystem / Device", 188);
            listView.Columns.Add("Detected Configuration", 330);

            // 1. CPU Architecture
            string processorArch = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE") ?? "Unknown";
            ListViewItem cpu = new ListViewItem("Processor (CPU)");
            cpu.SubItems.Add($"{processorArch} ({Environment.ProcessorCount} Cores)");

            // 2. Total Physical RAM (System-wide)
            string ramString = GetTotalMemoryInGB();
            ListViewItem ram = new ListViewItem("System Memory (RAM)");
            ram.SubItems.Add(ramString);

            // 3. Graphics (Placeholder or WMI query if needed)
            ListViewItem gpu = new ListViewItem("Graphics Subsystem");
            gpu.SubItems.Add("Detected via Display Adapter (Standard WMI can be added)");

            // 4. DirectX Runtime (Check registry or keep placeholder)
            ListViewItem directX = new ListViewItem("DirectX Runtime");
            directX.SubItems.Add("DirectX 11 / 12 Supported");

            // 5. Primary HDD Space
            string hddInfo = GetPrimaryDriveFreeSpace();
            ListViewItem hdd = new ListViewItem("Primary HDD Space");
            hdd.SubItems.Add(hddInfo);

            // 6. Operating System
            ListViewItem os = new ListViewItem("Operating Platform");
            os.SubItems.Add($"{Environment.OSVersion} ({Environment.OSVersion.Version})");

            listView.Items.Add(cpu);
            listView.Items.Add(ram);
            listView.Items.Add(gpu);
            listView.Items.Add(directX);
            listView.Items.Add(hdd);
            listView.Items.Add(os);
        }

        private string GetTotalMemoryInGB()
        {
            try
            {
                MEMORYSTATUSEX memStatus = new MEMORYSTATUSEX();
                if (GlobalMemoryStatusEx(memStatus))
                {
                    double totalGB = (double)memStatus.ullTotalPhys / (1024 * 1024 * 1024);
                    double availGB = (double)memStatus.ullAvailPhys / (1024 * 1024 * 1024);
                    return $"{totalGB:N1} GB total ({availGB:N1} GB free)";
                }
            }
            catch(Exception ex)
            {
                return ex.Message.ToString();
            }
            return "Unable to detect";
        }

        private string GetPrimaryDriveFreeSpace()
        {
            try
            {
                DriveInfo drive = new DriveInfo("C");
                if (drive.IsReady)
                {
                    double freeGB = (double)drive.AvailableFreeSpace / (1024 * 1024 * 1024);
                    double totalGB = (double)drive.TotalSize / (1024 * 1024 * 1024);
                    return $"Drive C: {freeGB:N1} GB Free of {totalGB:N1} GB";
                }
            }
            catch(Exception ex)
            {
                return ex.Message.ToString();
            }
            return "Information Unavailable";
        }
    }
}
