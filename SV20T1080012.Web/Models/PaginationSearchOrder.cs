using SV20T1080012.DomainModels;

namespace SV20T1080012.Web.Models
{
    public class PaginationSearchOrder : PaginationSearchBaseResult
    {
        public IList<Order>? Data { get; set; }
    }
}
