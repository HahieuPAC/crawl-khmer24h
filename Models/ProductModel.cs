using System.ComponentModel;

namespace CrawlDataWebsiteTool.Models
{
    public class ProductModel
    {
        [Description("Tên Sản Phẩm")] public string ProductName { get; set; }
        [Description("Giới Tính")] public string Gender { get; set; }
        [Description("Giá Gốc")] public string OrginPrice { get; set; }
        [Description("Giá Khuyến Mãi")] public string DiscountPrice { get; set; }
        [Description("Tiền Tệ")] public string Currency { get; set; }
        [Description("Link Sản Phẩm")] public string LinkDetail { get; set; }
        [Description("Tác Giả")] public string Author { get; set; }
    }
}
