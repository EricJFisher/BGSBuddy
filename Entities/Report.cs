namespace Entities
{
    public class Report
    {
        public string Location { get; set; }
        public string Situation { get; set; }
        public string Condition { get; set; }
        public string States { get; set; }

        public Report(string location, string situation, string condition, string states)
        {
            Location = location;
            Situation = situation;
            Condition = condition;
            States = states;
        }
    }
}
