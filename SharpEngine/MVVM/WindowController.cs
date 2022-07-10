using Prism.Commands;
using SharpEngine.dll_imports.structs;
using SharpEngine.memory;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
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
        public List<string> Log { get; set; }
        public event PropertyChangedEventHandler? PropertyChanged;
        public ProcessHandler handler;
        private NewProcess selectedProcess;
        private bool isSelected;
        private int selectedIndex;
        private List<string> readWriteSelection;
        private List<string> resultSelection;
        private string selectedMemory;
        private string selectedDatatype;

        private string resultText;
        private string memoryString;
        private string writeToAddress;


        public string ResultText { get => resultText; set => resultText = value; }
        public string MemoryString { get => memoryString; set => memoryString = value; }
        public List<string> ReadWriteSelection { get => readWriteSelection; set => readWriteSelection = value; }
        public List<string> ResultSelection { get => resultSelection; set => resultSelection = value; }


        public string WriteToAddress { get => writeToAddress; set => writeToAddress = value; }

        // event handling
        public DelegateCommand<object> listBoxSelectionChanged { get; set; }
        public DelegateCommand<object> reloadProcessesClick { get; set; }

        public DelegateCommand<object> startMemoryProcess { get; set; }
        public NewProcess SelectedProcess
        {
            get
            {
                return selectedProcess;
            }
            set
            {
                selectedProcess = value;
                isSelected = true;
            }
        }

        public string SelectedMemory
        {
            get
            {
                return selectedMemory;
            }
            set
            {
                selectedMemory = value;
            }
        }
        public string SelectedDataType
        {
            get
            {
                return selectedDatatype;
            }
            set
            {
                selectedDatatype = value;
            }
        }


        public int SelectedIndex
        {
            get
            {
                return selectedIndex;
            }
            set
            {
                selectedIndex = value;
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
                resetSelectonInListBox();
                new Thread(updateProcessesUI).Start();
            });

            startMemoryProcess = new DelegateCommand<object>((select) =>
            {
                // writing to address done
                if(SelectedProcess != null && SelectedDataType.Equals("Confirming") && SelectedMemory.Equals("Write"))
                {
                    if(handler.isAddressAssignedToProcess(SelectedProcess, MemoryString))
                    {
                        ResultText = Convert.ToString(handler.writeToMemory(SelectedProcess, MemoryString, WriteToAddress));
                        OnPropertyChanged(new PropertyChangedEventArgs("ResultText"));
                        Log.Add("Wrote " + WriteToAddress + "to Address: " + MemoryString);
                        OnPropertyChanged(new PropertyChangedEventArgs("Log"));
                    }
                    else
                    {
                        ResultText = "False: String is not a valid Address for Process.";
                        OnPropertyChanged(new PropertyChangedEventArgs("ResultText"));
                    }
                }
                // reading from address
                else if(SelectedProcess != null && SelectedDataType.Equals("Confirming") == false && SelectedMemory.Equals("Read"))
                {
                    if(handler.isAddressAssignedToProcess(SelectedProcess, MemoryString))
                    {
                        ResultText = Convert.ToString(handler.readFromMemory(SelectedProcess, MemoryString, SelectedMemory));
                        OnPropertyChanged(new PropertyChangedEventArgs("ResultText"));
                        Log.Add("Read " + ResultText + "from Address: " + MemoryString);
                        OnPropertyChanged(new PropertyChangedEventArgs("Log"));
                    }
                    else
                    {
                        ResultText = "False: String is not a valid Address for Process.";
                        OnPropertyChanged(new PropertyChangedEventArgs("ResultText"));
                    }
                }
            });

           
            isSelected = false;
            SelectedIndex = -1;
            fillReadWrite();
            fillDataTypeAndResult();
            handler = new ProcessHandler();
            Log = new List<string>();
            new Thread(updateProcessesUI).Start();
        }


        private void fillReadWrite()
        {
            readWriteSelection = new List<string>();
            ReadWriteSelection.Add("Read");
            ReadWriteSelection.Add("Write");
            OnPropertyChanged(new PropertyChangedEventArgs("ReadWriteSelection"));
        }

        private void fillDataTypeAndResult()
        {
            resultSelection = new List<string>();
            ResultSelection.Add("Confirming");
            ResultSelection.Add("String");
            ResultSelection.Add("Int");
            ResultSelection.Add("Float");
            OnPropertyChanged(new PropertyChangedEventArgs("ResultSelection"));

        }

        private void resetSelectonInListBox()
        {
            SelectedProcess = null;
            OnPropertyChanged(new PropertyChangedEventArgs("SelectedProcess"));
            SelectedIndex = -1;
            OnPropertyChanged(new PropertyChangedEventArgs("SelectedIndex"));
            isSelected = false;
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
