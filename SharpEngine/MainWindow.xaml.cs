using SharpEngine.memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SharpEngine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            new Thread(updateProcessesThread).Start();
        }

        public void updateProcessesThread()
        {
            while(true)
            {
                Thread.Sleep(1000);
                this.Dispatcher.Invoke(() =>
                {
                    listViewProcesses.ItemsSource = new ProcessHandler().AllProcesses;
                });
            }
        }




    }
}
