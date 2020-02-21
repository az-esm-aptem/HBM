using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

namespace HMB_Utility
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        TreeViewItem Connectors;
        TreeViewItem Channels;
        TreeViewItem Signals;
        HBM_Object logger;

        public MainWindow()
        {
            InitializeComponent();
            //DataContext = new ApplicationViewModel();
            logger = new HBM_Object();

        }

        private void SearchDeviceButton_Click(object sender, RoutedEventArgs e)
        {
            
            //Task searchTask = new Task(() => logger.SearchDevices());
            //searchTask.Start();
            //searchTask.Wait();
            
            if (logger.SearchDevices(3000, 20000))
            {
                if (logger.ConnectToFoundDevices(logger.deviceList))
                {
                    List<TreeViewItem> deviceTreeViewItems = new List<TreeViewItem>();
                    List<TreeViewItem> connectorTreeViewItems = new List<TreeViewItem>();
                    List<TreeViewItem> chanelTreeViewItems = new List<TreeViewItem>();
                    List<TreeViewItem> signalTreeViewItems = new List<TreeViewItem>();

                    //List<Signal> sl = logger.deviceList[0].GetAllSignals();

                    //List<Signal> signalToChangeName = new List<Signal> {sl[0]};
                    //Console.WriteLine(signalToChangeName[0].Name);



                    //foreach (Signal s in sl)
                    //{
                    //    Console.WriteLine(s.Name);
                    //}
                    //logger.deviceList[0].Connectors[0].Channels[0].Name = "CHANNEL222222";
                    //List<Problem> pl = new List<Problem>();
                    //logger.deviceList[0].AssignChannel(logger.deviceList[0].Connectors[0].Channels[0], out pl);

                    //foreach (Device dev in logger.deviceList)
                    //{
                    //    Console.WriteLine(dev.Name);
                    //    foreach (Connector con in dev.Connectors)
                    //    {
                    //        Console.WriteLine(con.LocationHint);
                    //        foreach (Channel ch in con.Channels)
                    //        {
                    //            Console.WriteLine(ch.Name);
                    //            foreach (Signal sig in ch.Signals)
                    //            {
                    //                Console.WriteLine(sig.Name);
                    //            }
                    //        }
                    //    }
                    //}

                    for (int i = 0; i < logger.deviceList.Count; i++)
                    {
                        
                        deviceTreeViewItems.Add(new TreeViewItem { Header = string.Format(logger.deviceList[i].Name + '(' + logger.deviceList[i].FamilyName + ')'), Name = string.Format("Device_{0}", i) });
                        Devices_TreeViewItem.Items.Add(deviceTreeViewItems[i]);
                        for (int j = 0; j < logger.deviceList[i].Connectors.Count; j++)
                        {
                            connectorTreeViewItems.Add(new TreeViewItem { Header = string.Format(logger.deviceList[i].Connectors[j].LocationHint), Name = string.Format("Connector_{0}", j) });
                            deviceTreeViewItems[i].Items.Add(connectorTreeViewItems[j]);
                            //for (int k = 0; k < logger.deviceList[i].Connectors[j].Channels.Count; k++)
                            //{
                            //    chanelTreeViewItems.Add(new TreeViewItem { Header = string.Format(logger.deviceList[i].Connectors[j].Channels[k].Name), Name = string.Format("Channel_{0}", k) });
                            //    connectorTreeViewItems[j].Items.Add(chanelTreeViewItems[k]);
                            //    for (int n = 0; n < logger.deviceList[i].Connectors[j].Channels[k].Signals.Count; n++)
                            //    {
                            //        signalTreeViewItems.Add(new TreeViewItem { Header = string.Format(logger.deviceList[i].Connectors[j].Channels[k].Signals[n].Name), Name = string.Format("Signal_{0}", n) });
                            //        chanelTreeViewItems[k].Items.Add(signalTreeViewItems[n]);
                            //    }
                            //}
                        }
                        deviceTreeViewItems.Clear();
                        connectorTreeViewItems.Clear();
                        chanelTreeViewItems.Clear();
                        signalTreeViewItems.Clear();
                    }
                }
            }
        }
    }
}
