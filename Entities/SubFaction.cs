using System;
using System.Collections.Generic;

namespace Entities
{
    public class SubFaction
    {
        public string Name { get; set; }
        public double Influence { get; set; }
        public List<string> ActiveStates { get; set; } = new List<string>();
        public List<string> PendingStates { get; set; } = new List<string>();
        public DateTime UpdatedOn { get; set; }
    }
}
