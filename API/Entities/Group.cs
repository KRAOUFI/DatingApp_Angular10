using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class Group
    {
        [Key]
        public string Name { get; set; }
        public ICollection<Connection> Connections { get; set; } = new List<Connection>();
        
        public Group()
        {
            // Entity a besoin de ce constructeur vide pour la création des tables
        }

        public Group(string name)
        {
            Name = name;
        }
    }
}
