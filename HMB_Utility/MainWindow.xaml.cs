using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

using Hbm.Api.Common;
using Hbm.Api.Common.Messaging;
using Hbm.Api.Common.Exceptions;
using Hbm.Api.Common.Entities;
using Hbm.Api.Common.Entities.Problems;
using Hbm.Api.Common.Entities.Connectors;
using Hbm.Api.Common.Entities.Channels;
using Hbm.Api.Common.Entities.Signals;
using Hbm.Api.Common.Entities.Filters;
using Hbm.Api.Common.Entities.ConnectionInfos;
using Hbm.Api.Common.Enums;
using Hbm.Api.Scan;
using Hbm.Api.Pmx;
using Hbm.Api.QuantumX;
using Hbm.Api.Mgc;

using DB;

namespace HMB_Utility
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        HBM_Object logger;
        public MainWindow()
        {
            InitializeComponent();
            logger = HBM_Object.GetInstance();
            Btn1.IsEnabled = false;
        }

        public async Task SearchAsync(int period = 2000, int searchTime = 30000)
        {
             await Task.Run(() => logger.SearchDevices(period, searchTime));
        }

        public async Task ConnectAsync(List<Device>devList)
        {
            await Task.Run(() => logger.ConnectToFoundDevices(devList));
        }

        public async Task SaveToBDAsync(List<Device> devList)
        {
            await Task.Run(() => DataToDB.SaveDevices(devList));
        }

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
            TB1.Clear();
            TB1.Text += "Searching...";

            await SearchAsync();
            await ConnectAsync(logger.deviceList);
            TB1.Clear();
            TB1.Text += "Devices";
            foreach (Device dev in logger.deviceList)
            {
                
                TB1.Text += Environment.NewLine + dev.Name;
                List<Signal> signals = dev.GetAllSignals();
                TB1.Text += Environment.NewLine + "Signals";
                foreach (Signal sig in signals)
                {
                    TB1.Text += Environment.NewLine + sig.Name;
                }
            }

            
            
           


        }

        private void Btn1_Click(object sender, RoutedEventArgs e)
        {

            HMB_Utility.Measure.GetMeasurmentValue(logger.deviceList, DataToDB.SaveSingleMeasurments);


            //ObservableCollection<Device> devices = new ObservableCollection<Device>(logger.deviceList);
            //overviewTree.ItemsSource = logger.deviceList;




            //ObservableCollection<DeviceToUI> devicesToUI = new ObservableCollection<DeviceToUI>();
            //foreach (Device dev in logger.deviceList)
            //{
            //    DeviceToUI devUI = new DeviceToUI
            //    {
            //        Name = dev.Name,
            //        IpAddress = (dev.ConnectionInfo as EthernetConnectionInfo).IpAddress,
            //        Model = dev.Model,
            //        SerialNo = dev.SerialNo,
            //        Signals = new ObservableCollection<Signal>(dev.GetAllSignals())
            //    };
            //    devicesToUI.Add(devUI);
            //}

            //overviewTree.ItemsSource = devicesToUI;

        }

        private void Btn2_Click(object sender, RoutedEventArgs e)
        {
            using (HBMContext db = new HBMContext())
            {
                var sig = db.Signals.Include(d => d.Values);
                foreach (var s in sig)
                {
                    Console.WriteLine(s.Name);
                   
                        foreach (var val in s.Values)
                        {
                            Console.WriteLine("{0}\t{1}", val.dateTime, val.MeasuredValue);
                        }
                    
                }
            }
        }

        private async void Btn3_Click(object sender, RoutedEventArgs e)
        {
            await SaveToBDAsync(logger.deviceList);
            Btn1.IsEnabled = true;
        }
    }
}
