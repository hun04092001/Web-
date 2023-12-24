namespace SV20T1080012.Web.Models
{
    public class PaginationSearchProductInput : PaginationSearchInput
    {
        public int SupplierID { get; set; } = 0;
        /// <summary>
        /// 
        /// </summary>
        public int CategoryID { get; set; } = 0;
        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }
    }
}
