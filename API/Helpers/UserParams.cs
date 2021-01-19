namespace API.Helpers
{
    public class UserParams
    {
        #region Params pour la pagination
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        
        private int _pageSize = 10;
        public int PageSize 
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
        #endregion
        
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