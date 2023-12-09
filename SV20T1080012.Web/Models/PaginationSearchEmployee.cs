using SV20T1080012.DomainModels;

namespace SV20T1080012.Web.Models
{
    public class PaginationSearchEmployee : PaginationSearchBaseResult
    {
        public IList<Employee> Data { get; set; }
    }
}
