using SharpEngine.dll_imports.structs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SharpEngine.memory
{
    public class ProcessHandler
    {

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        [DllImport("kernel32.dll")]
        static extern void GetSystemInfo(out SYSTEM_INFO lpSystemInfo);


        const int PROCESS_QUERY_INFORMATION = 0x0400;
        const int MEM_COMMIT = 0x00001000;
        const int PAGE_READWRITE = 0x04;
        const int PROCESS_WM_READ = 0x0010;

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWow64Process([In] IntPtr hProcess, [Out] out bool lpSystemInfo);


        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(IntPtr hProcess,
    IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);
        public static bool Is64Bit(IntPtr process)
        {
            bool retVal;

            IsWow64Process(process, out retVal);

            return retVal;
        }

        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern
    bool CloseHandle(IntPtr handle);


        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);

        private int convertDatatypeToBytes(string datatype)
        {
            if(datatype.Equals("Int") || datatype.Equals("Float"))
            {
                return 4;
            }
            return 30;
        }
        public object readFromMemory(NewProcess process, string value, string dataTypeOption)
        {
            Process pro = process.Process;
            IntPtr handle = OpenProcess(PROCESS_QUERY_INFORMATION | 0xFFFF, false, pro.Id);
            long holdingActualNumber = 0;
            try
            {
                if (IsDigitsOnly(value) == true)
                {
                    holdingActualNumber = Convert.ToInt64(value);
                }
                else if (OnlyHexInString(value) == true)
                {
                    holdingActualNumber = Convert.ToInt64(value, 16);
                }
            }
            catch (Exception error)
            {
                return false;
            }
            IntPtr number = (IntPtr)holdingActualNumber;
            byte[] buffer = new byte[convertDatatypeToBytes(dataTypeOption)];
            IntPtr bytesRead = new IntPtr(0);
            ReadProcessMemory(handle, number, buffer, buffer.Length, out bytesRead);
            if(dataTypeOption.Equals("Int"))
            {
                return BitConverter.ToInt32(buffer, 0);
            }
            else if(dataTypeOption.Equals("Float"))
            {
                return BitConverter.ToDouble(buffer, 0);
            }
            return System.Text.Encoding.Default.GetString(buffer);

        }

        public bool writeToMemory(NewProcess process, string value, string writingContent)
        {
            Process pro = process.Process;
            IntPtr handle = OpenProcess(PROCESS_QUERY_INFORMATION | 0xFFFF, false, pro.Id);
            long holdingActualNumber = 0;
            try
            {
                if (IsDigitsOnly(value) == true)
                {
                    holdingActualNumber = Convert.ToInt64(value);
                }
                else if (OnlyHexInString(value) == true)
                {
                    holdingActualNumber = Convert.ToInt64(value, 16);
                }
            }
            catch (Exception error)
            {
                return false;
            }
            IntPtr number = (IntPtr)holdingActualNumber;
            byte[] buffer = Encoding.ASCII.GetBytes(writingContent);
            int bytesWritten = 0;
            return WriteProcessMemory(handle, number, buffer, buffer.Length, ref bytesWritten);
        }
        public bool isAddressAssignedToProcess(NewProcess process, string value)
        {

            // as we are passing out memory address as string, we need to check if its not a wrong one (formatting)
            long holdingActualNumber = 0;
            try
            {
                if (IsDigitsOnly(value) == true)
                {
                    holdingActualNumber = Convert.ToInt64(value);
                }
                else if (OnlyHexInString(value) == true)
                {
                    holdingActualNumber = Convert.ToInt64(value, 16);
                }
            }
            catch(Exception error)
            {
                return false;
            }
           

            // We receive start and end of allocated RAM in the host system.
            SYSTEM_INFO sysInfo = new SYSTEM_INFO();
            GetSystemInfo(out sysInfo);

            IntPtr min = sysInfo.minimumApplicationAddress;
            IntPtr max = sysInfo.maximumApplicationAddress;

            // conversions to add
            long min_l = (long)min;
            long max_l = (long)max;

            // get process out of new process
            Process pro = process.Process;

            IntPtr processHandle = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_WM_READ, false, pro.Id);

            long MaxAddress = max_l;
            long address = min_l;
            do
            {
                MEMORY_BASIC_INFORMATION m;
                int result = VirtualQueryEx(processHandle, (IntPtr)address, out m, (uint)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION)));
                long regionsize = (long)m.RegionSize-1;
                long start = (long)m.BaseAddress;

                if (holdingActualNumber >= start && holdingActualNumber <= start + regionsize)
                {
                    return true;
                }

                // add chunk to address to skip thiis page
                if (address == (long)m.BaseAddress + (long)m.RegionSize)
                    break;
                address = (long)m.BaseAddress + (long)m.RegionSize;
            } while (address <= MaxAddress);


            CloseHandle(processHandle);
            return false;
        }

        private bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        private bool OnlyHexInString(string test)
        {
            // For C-style hex notation (0xFF) you can use @"\A\b(0[xX])?[0-9a-fA-F]+\b\Z"
            return System.Text.RegularExpressions.Regex.IsMatch(test, @"\A\b[0-9a-fA-F]+\b\Z");
        }
        public ObservableCollection<NewProcess> getActiveProcesses()
        {
            ObservableCollection<NewProcess> processes = new ObservableCollection<NewProcess>();

            Process[] processCollection = Process.GetProcesses().Where(x => x.MainWindowHandle != IntPtr.Zero).ToArray();
            foreach (Process p in processCollection)
            {
                Icon ico = null;
                try
                {
                    ico = Icon.ExtractAssociatedIcon(p.MainModule.FileName);
                }
                catch (Exception error)
                {
                    Console.WriteLine(error.Message);
                }
                ImageSource source = Imaging.CreateBitmapSourceFromHIcon(ico.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                processes.Add(new NewProcess(p, ico, source));

            }
            return processes;
        }
    }
}
