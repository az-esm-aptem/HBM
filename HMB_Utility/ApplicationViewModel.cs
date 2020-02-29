using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace HMB_Utility
{
    public class ApplicationViewModel : ViewModelBase
    {
        private FoundDevice _selectedDevice;
        private ObservableCollection<FoundDevice> _devs;
        public ObservableCollection<FoundDevice> Devs { get; set; }
        
        public FoundDevice SelectedDevice
        {
            get
            {
                return _selectedDevice;
            }
            set
            {
                _selectedDevice = value;
                OnPropertyChanged("SelectedDevice");
            }
        }

       
        public ApplicationViewModel(List<FoundDevice>devList)
        {
            Devs = new ObservableCollection<FoundDevice>(devList);
        }

    }
}
