namespace Entities
{
    public class InvasionReport
    {
        public string SystemName { get; set; }
        public string TicksOld { get; set; }
        public string ControllingFaction { get; set; }
        public string TargetFaction { get; set; }
        public double Influence { get; set; }
    }
}
