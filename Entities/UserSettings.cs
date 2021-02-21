using System.Collections.Generic;

namespace Entities
{
    public class UserSettings
    {
        public string FactionName { get; set; } = string.Empty;
        public List<string> OffLimits { get; set; } = new List<string>();
        public bool ShowNonNativeConflictReports { get; set; } = false;
    }
}
