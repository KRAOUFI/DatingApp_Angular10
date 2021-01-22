namespace API.Entities
{
    public class Like
    {
        public User Source { get; set; }
        public int SourceId { get; set; }
        public User Liked { get; set; }
        public int LikedId { get; set; }

    }
}