using System.Collections.ObjectModel;

namespace HMB_Utility
{
    public class DeviceToUI
    {
        public string Name { get; set; }
        public string IpAddress { get; set; }
        public string Model { get; set; }
        public string SerialNo { get; set; }
        public ObservableCollection<Hbm.Api.Common.Entities.Signals.Signal> Signals { get; set; }
        public DeviceToUI()
        {
            Signals = new ObservableCollection<Hbm.Api.Common.Entities.Signals.Signal>();
        }
    }
}
