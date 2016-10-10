using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheWorld.Models
{
    public class Trip
    {
        // Unique Identify like a PK. Like database fields
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public string UserName { get; set; }

        //ICollection allows add/remove. IEnumerable is read only
        public ICollection<Stop> Stops { get; set; }

    }
}
