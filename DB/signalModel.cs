using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace DB
{
    public class SignalModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UniqueId { get; set; }
        public decimal SampleRate { get; set; }
        public ICollection<ValuesModel> Values { get; set; }
        public int? DeviceId { get; set; }

        [ForeignKey("DeviceId")]
        public DeviceModel Device { get; set; }

        public SignalModel()
        {
            Values = new List<ValuesModel>();
        }
    }
}
