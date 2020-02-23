namespace DB
{
    public class ValuesModel
    {
        public int Id { get; set; }
        public System.DateTime dateTime { get; set; }
        public double MeasuredValue { get; set; }
        public int State { get; set; }
        public double TimeStamp { get; set; }
        public int? SignalId { get; set; }
        public SignalModel Signal { get; set; }


    }
}
