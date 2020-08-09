using System.Collections.Generic;

namespace Entities
{
    public class Conflict
    {
        public string Type { get; set; }
        public string Status { get; set; }
        public List<ConflictFaction> Factions { get; set; } = new List<ConflictFaction>();
    }
}
