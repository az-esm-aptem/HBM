namespace DB
{
    public class ValuesModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double MeasuredValue { get; set; } 
        public int? SignalId { get; set; }
        public SignalModel Signal { get; set; }


    }
}
