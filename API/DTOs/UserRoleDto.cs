using System.Collections.Generic;

namespace API.DTOs
{
    public class UserRoleDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public List<string> Roles { get; set; }
    }
}