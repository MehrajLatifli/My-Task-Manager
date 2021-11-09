using My_Task_Manager.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace My_Task_Manager.View_Models
{
    public class MainViewModel : BaseViewModel
    {
        public MainWindow MainWindows { get; set; }

        public RelayCommand SelectProcessCommand { get; set; }

        public RelayCommand EndProcessCommand { get; set; }

        public RelayCommand CreateProcessCommand { get; set; }

        public RelayCommand AddBlackListCommand { get; set; }

        public RelayCommand SearchCommand { get; set; }



        private Process _Process;
        public Process Process { get { return _Process; } set { _Process = value; OnPropertyChanged(); } }



        private PerformanceCounter _PerformanceCounter1;
        public PerformanceCounter PerformanceCounter1 { get { return _PerformanceCounter1; } set { _PerformanceCounter1 = value; OnPropertyChanged(); } }

        private PerformanceCounter _PerformanceCounter2;
        public PerformanceCounter PerformanceCounter2 { get { return _PerformanceCounter2; } set { _PerformanceCounter2 = value; OnPropertyChanged(); } }




        private ObservableCollection<Process> allProcess;

        public ObservableCollection<Process> AllProcess
        {
            get { return allProcess; }
            set { allProcess = value; OnPropertyChanged(); }
        }

        public MainViewModel()
        {


            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += Timer_Tick; ;
            timer.Interval = TimeSpan.FromSeconds(1);


            AllProcess = new ObservableCollection<Process>(Process.GetProcesses());



            timer.Start();






            EndProcessCommand = new RelayCommand((sender) =>
            {
                try
                {


                    if (MainWindows.AddTaskTxt.Text == string.Empty)
                    {
                        foreach (var item in AllProcess)
                        {
                            Console.WriteLine($"{item.Id}  {item.ProcessName}");

                            var i = MainWindows.proceslistview.SelectedItem as Process;

                            if (item.ProcessName == i.ProcessName)
                            {
                                if (!item.WaitForExit(1000))
                                {


                                    MessageBox.Show(item.StartTime.ToLongTimeString());
                                    for (int J = 0; J < 100; J++)
                                    {
                                        if (!item.HasExited) item.Kill();

                                    }

                                }
                            }

                        }

                        AllProcess = new ObservableCollection<Process>(Process.GetProcesses());
                    }
                }
                catch (Exception)
                {


                }
            });


            SearchCommand = new RelayCommand((sender) =>
            {
                try
                {


                    MainWindows.proceslistview.ItemsSource = null;

                    if (string.IsNullOrEmpty(MainWindows.SearchTXT.Text) == false)
                    {
                        MainWindows.proceslistview.ItemsSource = null;
                        MainWindows.proceslistview.Items.Clear();

                        foreach (var item in AllProcess)
                        {
                            if (item.ProcessName.StartsWith(MainWindows.SearchTXT.Text))
                            {
                                MainWindows.proceslistview.Items.Add(item);
                            }
                            MainWindows.proceslistview.ItemsSource = null;

                        }

                        AllProcess = new ObservableCollection<Process>(Process.GetProcesses());


                    }

                    else
                    {
                        MainWindows.proceslistview.Items.Clear();

                        foreach (var item in AllProcess)
                        {

                            MainWindows.proceslistview.Items.Add(item);

                        }
                        AllProcess = new ObservableCollection<Process>(Process.GetProcesses());
                    }
                }
                catch (Exception)
                {


                }

            });


            CreateProcessCommand = new RelayCommand((sender) =>
            {
                try
                {

                    foreach (var item in AllProcess)
                    {
                        Process p = new Process();
                        p.StartInfo.FileName = MainWindows.AddTaskTxt.Text;
                        p.Start();

                        AllProcess.Add(p);

                    }
                    AllProcess = new ObservableCollection<Process>(Process.GetProcesses());

                }
                catch (Exception)
                {


                }

                MainWindows.AddTaskTxt.Text = string.Empty;
            });


            AddBlackListCommand = new RelayCommand((sender) =>
            {

                MainWindows.blacklistbox.Items.Add(MainWindows.BlacklistTxt.Text);


                try
                {

                    foreach (var item in MainWindows.blacklistbox.Items)
                    {
                        var processes = AllProcess.Where(p => p.ProcessName == item.ToString());


                        if (processes != null)
                        {

                            foreach (var item2 in processes)
                            {
                                Console.WriteLine($"{item2.Id}  {item2.ProcessName}");


                                var process = AllProcess.FirstOrDefault(p => p.ProcessName == item.ToString());

                                if (item2.ProcessName == process.ProcessName)
                                {
                                    if (!item2.WaitForExit(1000))
                                    {


                                        MessageBox.Show(item2.StartTime.ToLongTimeString());
                                        for (int J = 0; J < 100; J++)
                                        {
                                            if (!item2.HasExited) item2.Kill();

                                        }

                                    }
                                }

                            }

                            AllProcess = new ObservableCollection<Process>(Process.GetProcesses());

                        }


                    }

                }
                catch (Exception)
                {


                }
            });


        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            PerformanceCounter1 = new PerformanceCounter("Processor", "% Idle Time", "_Total");

            PerformanceCounter2 = new PerformanceCounter("Memory", "% Committed Bytes In Use");




            float cpu = PerformanceCounter1.NextValue();



            MainWindows.CPU_tbx.Text = string.Format("{0:0.00} %", PerformanceCounter1.NextValue());
            MainWindows.RAM_tbx.Text = string.Format("{0:0.00} %", PerformanceCounter2.NextValue());
        }
    }
}
