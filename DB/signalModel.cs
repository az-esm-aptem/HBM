using System.Collections.Generic;


namespace DB
{
    public class SignalModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal SampleRate { get; set; }
        public ICollection<ValuesModel> Values { get; set; }
        public int? DeviceModelId { get; set; }
        public DeviceModel Device { get; set; }

        public SignalModel()
        {
            Values = new List<ValuesModel>();
        }
    }
}
