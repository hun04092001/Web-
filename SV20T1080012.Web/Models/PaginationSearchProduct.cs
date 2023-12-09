using SV20T1080012.DomainModels;

namespace SV20T1080012.Web.Models
{
    public class PaginationSearchProduct : PaginationSearchBaseResult
    {
        public IList<Product> Data { get; set; }
    }
}
