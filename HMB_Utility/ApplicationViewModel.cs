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
    class ApplicationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
       
        public ObservableCollection<FoundDevice> devices;
        private FoundDevice _selectedDevice;
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
            devices = new ObservableCollection<FoundDevice>(devList);
        }

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
