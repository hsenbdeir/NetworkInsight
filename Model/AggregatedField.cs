namespace NetworkInsight.Model
{
    public class AggregatedField
    {
        public DateTime Time { get; set; }
        public string NeType { get; set; }
        public string NeAlias { get; set; }
        public double RFInputPower { get; set; }
        public double MaxRxLevel { get; set; }
        public double RSL_Deviation { get; set; }
    }
}
