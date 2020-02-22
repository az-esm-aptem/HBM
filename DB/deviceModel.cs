using System.Collections.Generic;


namespace DB
{
    public class DeviceModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string IpAddress { get; set; }
        public string Model { get; set; }
        public string SerialNo { get; set; }
        public ICollection<SignalModel> Signals { get; set; }

        public DeviceModel()
        {
            Signals = new List<SignalModel>();
        }


    }
}
