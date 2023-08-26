using System.ComponentModel;

namespace CrawlDataWebsiteTool.Models
{
    public class ProductModel
    {
        [Description("ID")] public string ID { get; set; }
        [Description("Category")] public string Category { get; set; }
        [Description("Locations")] public string Locations { get; set; }
        [Description("Posted")] public string Posted { get; set; }
        [Description("Product's name")] public string ProductName { get; set; }
        [Description("Product Price")] public string Price { get; set; }
        [Description("Car Makes")] public string CarMakes { get; set; }
        [Description("Tax Type")] public string TaxType { get; set; }
        [Description("Color")] public string Color { get; set; }
        [Description("Car Mode")] public string CarModel { get; set; }
        [Description("Condition")] public string Condition { get; set; }
        [Description("Year")] public string Year { get; set; }
        [Description("Transmission")] public string Transmission { get; set; }
        [Description("Phone number 1")] public string PhoneNumber0 { get; set; }
        [Description("Phone number 2")] public string PhoneNumber1 { get; set; }
        [Description("Phone number 3")] public string PhoneNumber2 { get; set; }
    }
}
