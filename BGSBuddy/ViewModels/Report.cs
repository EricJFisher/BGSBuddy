namespace BGSBuddy.ViewModels
{
    public class Report
    {
        public string Location { get; set; }
        public string Situation { get; set; }
        public string Condition { get; set; }

        public Report(string location, string situation, string condition)
        {
            Location = location;
            Situation = situation;
            Condition = condition;
        }
    }
}
