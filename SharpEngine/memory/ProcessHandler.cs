using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
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
