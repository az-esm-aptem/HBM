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

        public MainWindow()
        {
            InitializeComponent();
            

        }

        private void SearchDeviceButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 1; i < 5; i++)
            {
                Devices_TreeViewItem.Items.Add(new TreeViewItem { Header = string.Format("Device - {0}", i), Name = string.Format("Device_{0}", i) });

            }
        }
    }
}
