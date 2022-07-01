using SharpEngine.dll_imports.enums;
using SharpEngine.dll_imports.structs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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


        private List<NewProcess> allProcesses;

        public ProcessHandler()
        {
            allProcesses = new List<NewProcess>();
            ListProcesses();
            test();
        }


        public void test()
        {
            dotNetMemoryScan dot = new dotNetMemoryScan();
            var test1 = IntPtr.Zero;
            var test2 = IntPtr.Zero;
        }
        internal List<NewProcess> AllProcesses { get => allProcesses; set => allProcesses = value; }


        private void ListProcesses()
        {
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
                allProcesses.Add(new NewProcess(p, ico, source));

            }

        }
    }
}
