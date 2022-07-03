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
        private NewProcess selectedProcess;
        private bool isSelected;


        // event handling
        public DelegateCommand<object> listBoxSelectionChanged { get; set; }
        public DelegateCommand<object> reloadProcessesClick { get; set; }
        public NewProcess SelectedProcess
        {
            get
            {
                MessageBox.Show("getter called!");
                return selectedProcess;
            }
            set
            {
                selectedProcess = value;
                isSelected = true;
                MessageBox.Show("setter called!");
            }
        }

        public WindowController()
        {
            // event handling
            listBoxSelectionChanged = new DelegateCommand<object>((selecteditem) =>
            {
                
                
                
            });

            reloadProcessesClick = new DelegateCommand<object>((selected) =>
            {
                isSelected = false;
                SelectedProcess = null;
                new Thread(updateProcessesUI).Start();
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
            while(isSelected == false)
            {
                if(ActiveProcesses != null && isSelected == false)
                {
                    Thread.Sleep(1000);
                }
                Application.Current.Dispatcher.Invoke(() => 
                {
                    if(isSelected == false)
                    {
                        ActiveProcesses = handler.getActiveProcesses();
                        OnPropertyChanged(new PropertyChangedEventArgs("ActiveProcesses"));
                    }
                });
                

            }
        }


        


    }
}
