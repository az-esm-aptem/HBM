using System.Collections.ObjectModel;

namespace HMB_Utility
{
    class ChannelToUI
    {
        public string Name { get; set; }
        public ObservableCollection<Hbm.Api.Common.Entities.Signals.Signal> Signals { get; set; }
        public ChannelToUI()
        {
            Signals = new ObservableCollection<Hbm.Api.Common.Entities.Signals.Signal>();
        }
    }
}
