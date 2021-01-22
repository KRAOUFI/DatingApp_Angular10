namespace API.Helpers
{
    public class UserParams : PaginationParams
    {   
        #region Params pour les critères/filtres de recherche
        public string CurrentUsername { get; set; }
        public string Gender  { get; set; }
        public int MinAge { get; set; } = 18;
        public int MaxAge { get; set; } = 150;
        #endregion
        
        #region Params pour le Tri
        public string OrderBy { get; set; } = "lastActive";
        #endregion
    }
}