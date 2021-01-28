namespace API.Entities
{
    public class Like
    {
        public AppUser Source { get; set; }
        public int SourceId { get; set; }
        public AppUser Liked { get; set; }
        public int LikedId { get; set; }

    }
}