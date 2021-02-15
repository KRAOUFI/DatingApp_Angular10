namespace API.Entities
{
    public class Connection
    {
        
        public string ConnectionId { get; set; }
        public string Username { get; set; }

        public Connection()
        {
            // Entity a besoin de ce constructeur vide pour la création des tables
        }

        public Connection(string connectionId, string username)
        {
            ConnectionId = connectionId;
            Username = username;
        }
    }
}
