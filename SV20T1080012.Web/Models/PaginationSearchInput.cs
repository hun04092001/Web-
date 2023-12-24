namespace SV20T1080012.Web.Models
{
    /// <summary>
    /// lưu trữ thông tin đầu vào để tìm kiếm , phân  trang
    /// </summary>
    public class PaginationSearchInput
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string SearchValue { get; set; } = "";
    }
}
