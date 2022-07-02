using Prism.Commands;
using SharpEngine.memory;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SharpEngine.MVVM
{
    public class WindowController : INotifyPropertyChanged
    {
        public ObservableCollection<NewProcess> ActiveProcesses { get; set; }
        public event PropertyChangedEventHandler? PropertyChanged;
        public ProcessHandler handler;


        // event handling
        public DelegateCommand<object> listBoxSelectionChanged { get; set; }
        
        
        public WindowController()
        {
            // event handling
            listBoxSelectionChanged = new DelegateCommand<object>((selecteditem) =>
            {
                MessageBox.Show("wow it changed!");
            });


            handler = new ProcessHandler();
            new Thread(updateProcessesUI).Start();
        }

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        private void updateProcessesUI()
        {
            while(true)
            {
                if(ActiveProcesses != null)
                {
                    Thread.Sleep(1000);
                }
                Application.Current.Dispatcher.Invoke(() => 
                {
                    ActiveProcesses = handler.getActiveProcesses();
                    OnPropertyChanged(new PropertyChangedEventArgs("ActiveProcesses"));
                });
                

            }
        }


        


    }
}
