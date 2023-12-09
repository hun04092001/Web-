using SV20T1080012.DomainModels;

namespace SV20T1080012.Web.Models
{
    
    public class PaginationSearchCustomer : PaginationSearchBaseResult
    {
        public IList<Customer> Data { get; set; }
    }
}
