using System.Collections.ObjectModel;


namespace HMB_Utility
{
    class ConnectorToUI
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public ObservableCollection<Hbm.Api.Common.Entities.Channels.Channel> Channels { get; set; }
        public ConnectorToUI()
        {
            Channels = new ObservableCollection<Hbm.Api.Common.Entities.Channels.Channel>();
        }
    }
}
