using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    /// <summary>
    /// Par defaut, IdentityUser utilise un string pour la clé primaire. Ici nous lui indiquons que la clé primaire est un integer
    /// Aussi nous n'avons pas besoin de définir des propriétés comme ID, UserName, PasswordHash ou PasswordSalt
    /// parce que ces champs viennent avec l'implementation de IdentityUser.
    /// </summary>
    public class AppUser : IdentityUser<int>
    {
        public DateTime DateOfBirth { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime LastActive { get; set; } = DateTime.Now;
        public string Gender { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public ICollection<Photo> Photos { get; set; }
        
        public ICollection<Like> LikedBy { get; set; }
        public ICollection<Like>  Liked { get; set; }
        public ICollection<Message> MessageSent { get; set; }
        public ICollection<Message> MessageReceived { get; set; }
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}
