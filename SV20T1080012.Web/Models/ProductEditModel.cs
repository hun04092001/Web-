using SV20T1080012.DomainModels;

namespace SV20T1080012.Web.Models
{
    public class ProductEditModel
    {
        /// <summary>
        /// lấy ra danh sách mặt hàng
        /// </summary>
        public Product Product { get; set; }
        /// <summary>
        /// lấy ra danh sách các thuộc tính của mặt hàng 
        /// </summary>
        public List<ProductAttribute> ProductAttributes { get; set; }
        /// <summary>
        /// lấy ra danh sách ảnh của mặt hàng
        /// </summary>
        public List<ProductPhoto> ProductPhotos { get; set; }
    }
}
