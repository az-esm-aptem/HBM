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

        }

        public async void SearchAsync(int period = 3000, int searchTime = 30000)
        {
            bool result = await Task.Run(() => logger.SearchDevices(period, searchTime));
        }

        private void SearchDeviceButton_Click(object sender, RoutedEventArgs e)
        {
            //SearchAsync();

            using (HBMContext db = new HBMContext())
            {
                db.Devices.Add(new DeviceModel { Name = "Test device", IpAddress = "192.168.0.0" });
                db.SaveChanges();
            }


        }
    }
}
