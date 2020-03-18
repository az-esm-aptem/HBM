using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;





namespace HMB_Utility
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        MainWindowViewModel mainWindowViewModel;
        public MainWindow()
        {
            InitializeComponent();
            mainWindowViewModel = new MainWindowViewModel();
            DataContext = mainWindowViewModel;
            ((INotifyCollectionChanged)Protocol.Items).CollectionChanged += ListView_CollectionChanged;
        }


        private void ListView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                // scroll the new item into view   
                Protocol.ScrollIntoView(e.NewItems[0]);
            }
        }

        private void SignalList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (FoundSignal s in e.AddedItems)
            {
                if (!mainWindowViewModel.SelectedDevice.SignalsToMeasure.Contains(s))
                {
                    mainWindowViewModel.SelectedDevice.SignalsToMeasure.Add(s);
                }
            }
            foreach (FoundSignal s in e.RemovedItems)
            {
                if (mainWindowViewModel.SelectedDevice.SignalsToMeasure.Contains(s))
                {
                    mainWindowViewModel.SelectedDevice.SignalsToMeasure.Remove(s);
                }
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (FoundSignal s in e.AddedItems)
            {
                if (!mainWindowViewModel.SelectedDevice.SignalsToMeasure.Contains(s))
                {
                    mainWindowViewModel.SelectedDevice.SignalsToMeasure.Add(s);
                }
            }
            foreach (FoundSignal s in e.RemovedItems)
            {
                if (mainWindowViewModel.SelectedDevice.SignalsToMeasure.Contains(s))
                {
                    mainWindowViewModel.SelectedDevice.SignalsToMeasure.Remove(s);
                }
            }
        }

      























        /*
        public void ShowDevices()
        {
            using (HBMContext db = new HBMContext())
            {
                var dev = db.Devices.Include(d => d.Signals).ToList();
                foreach (var d in dev)
                {
                    Console.WriteLine(d.Name);
                    foreach (var s in d.Signals)
                    {
                        Console.WriteLine(s.Name);
                    }
                }
            }
        }

        private async void SearchDeviceButton_Click(object sender, RoutedEventArgs e)
        {
            //devices.Clear();
            //TB1.Clear();
            //TB1.Text += "Searching...";
            //await session.SearchAsync();
            //foreach (Device dev in session.deviceList)
            //{
            //    devices.Add(new FoundDevice(dev));
            //}
            //TB1.Clear();
            //TB1.Text += "DEVICES";
            //foreach (FoundDevice dev in devices)
            //{
            //    TB1.Text += Environment.NewLine + dev.Name;
            //}
            
        }


        private async void Connect_btn_Click(object sender, RoutedEventArgs e)
        {
            TB1.Clear();
            TB1.Text += "Connecting...";
            if (await session.ConnectAsync(devices))
            {
                TB1.Clear();
                TB1.Text += "DEVICES";
                foreach (FoundDevice dev in devices)
                {
                    TB1.Text += Environment.NewLine + dev.Name;
                    TB1.Text += Environment.NewLine + "Signals";
                    foreach (Signal sig in dev.Signals)
                    {
                        TB1.Text += Environment.NewLine + sig.Name;
                    }
                }
            }
            else
            {
                TB1.Text += Environment.NewLine + "Connection Error";
            }

        }

        private async void Btn1_Click(object sender, RoutedEventArgs e)
        {
            PrepareSignalList();
            await Measuring.GetMeasurmentValueAsync(devices, DataToDB.SaveSingleMeasurments);
        }

        private void Btn2_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private async void Btn3_Click(object sender, RoutedEventArgs e)
        {
            await DataToDB.SaveDevicesAsync(devices);
            Btn1.IsEnabled = true;
        }

     
        private async void Btn4_Click(object sender, RoutedEventArgs e)
        {
            PrepareSignalList();
            foreach (var dev in devices)
            {
                DaqSessions.Add(new DAQ(dev, DataToDB.SaveDAQMeasurments));
            }
            foreach (DAQ daq in DaqSessions)
            {
               await daq.StartAsync(3000, 1);
            }
            Btn4.IsEnabled = false;
            Btn5.IsEnabled = true;
        }

        private void Btn5_Click(object sender, RoutedEventArgs e)
        {
            foreach (DAQ daq in DaqSessions)
            {
                daq.Stop();
                Btn5.IsEnabled = false;
                Btn4.IsEnabled = true;
            }
        }

        private void PrepareSignalList()
        {
            foreach (var dev in devices)
            {
                dev.SignalsToMeasure.Clear();
                foreach (var sig in dev.Signals)
                {
                    if (TypeFilter.Check(sig)) dev.SignalsToMeasure.Add(sig);
                }
            }
        }
        
        */
    }
}
