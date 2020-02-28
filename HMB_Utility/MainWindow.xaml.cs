﻿using System;
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
        HbmSession session;
        List<DAQ> DaqSessions;
        List<FoundDevice> devices;
        public MainWindow()
        {
            InitializeComponent();
            session = HbmSession.GetInstance();
            DaqSessions = new List<DAQ>();
            Btn1.IsEnabled = false;
            devices = new List<FoundDevice>();
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
            devices.Clear();
            TB1.Clear();
            TB1.Text += "Searching...";
            await session.SearchAsync();
            foreach (Device dev in session.deviceList)
            {
                devices.Add(new FoundDevice(dev));
            }
            TB1.Clear();
            TB1.Text += "DEVICES";
            foreach (FoundDevice dev in devices)
            {
                TB1.Text += Environment.NewLine + dev.Name;
            }
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
                    foreach (Signal sig in dev.signals)
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

        private void Btn1_Click(object sender, RoutedEventArgs e)
        {

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
            //await SaveToBDAsync(session.deviceList);
            Btn1.IsEnabled = true;
        }

     
        private void Btn4_Click(object sender, RoutedEventArgs e)
        {
            
            foreach(Device dev in session.deviceList)
            {
                //DaqSessions.Add(new DAQ(dev, DataToDB.SaveDAQMeasurments));
            }
            foreach (DAQ daq in DaqSessions)
            {
                //daq.Start(3000, 1);
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

        
    }
}
