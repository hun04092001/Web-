using SV20T1080012.DomainModels;

namespace SV20T1080012.Web.Models
{
    public class PaginationSearchCategory : PaginationSearchBaseResult
    {
        public IList<Category> Data { get; set; }
    }
}
