using SV20T1080012.DomainModels;

namespace SV20T1080012.Web.Models
{
    public class PaginationSearchSuppliers : PaginationSearchBaseResult
    {
        public IList<Supplier> Data { get; set; }
    }
}
