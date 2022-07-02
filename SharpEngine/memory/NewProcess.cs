using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SharpEngine.memory
{
    public class NewProcess
    {
        private Process process;
        private Icon icon;
        private ImageSource image;
        private string processName;

        public NewProcess(Process process, Icon icon, ImageSource image)
        {
            this.process = process;
            this.icon = icon;
            this.image = image;
            processName = " {" + process.PrivateMemorySize64.ToString("X") + "} " + process.ProcessName + ".exe";

        }

        public Process Process { get => process; set => process = value; }
        public Icon Icon { get => icon; set => icon = value; }
        public ImageSource Image { get => image; set => image = value; }
        public string ProcessName { get => processName; set => processName = value; }

        public override string ToString()
        {
            return image + " | " + process.ProcessName + ".exe";
        }
    }
}
