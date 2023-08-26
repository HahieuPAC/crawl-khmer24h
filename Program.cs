using CrawlDataWebsiteTool.Models;
using CrawlDataWebsiteToolBasic.Functions;
using CrawlDataWebsiteToolBasic.Helpers;
using Fizzler.Systems.HtmlAgilityPack;
using CrawlDataWebsiteToolBasic.ExportTool;
using HtmlAgilityPack;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using static System.Net.WebRequestMethods;

/// <summary>
/// 
/// Data will be crawling from this website: https://fridayshopping.vn
/// You need to have basic knowledge with C#, Web HTML, CSS
/// If have any bug or question. Please comment in following this link: https://www.code-mega.com/p?q=crawl-data-trich-xuat-du-lieu-website-voi-c-phan-1-2c222jN
/// Advanced Tools here: https://www.code-mega.com/p?q=crawl-data-trich-xuat-du-lieu-website-voi-c-phan-2-72953tZ
/// 
/// </summary>
/// 
/// <param name="currentPath"> Get curent path of project | Lấy đường dẫn của chương trình </param>
/// <param name="savePathExcel"> Path save excel file | Đường dẫn để lưu file excel </param>
/// <param name="baseUrl"> URL website need to crawl | Đường dẫn trang web cần crawl </param>
/// 


internal class Program
{
    public static string GetInnerTextSafely(List<HtmlNode> nodeList, int index)
    {
        if (index >= 0 && index < nodeList.Count)
        {
            return nodeList[index].InnerText.RemoveBreakLineTab();
        }
        return "";
    }
    public static string GetPhoneNumberSafely(List<string> phoneNumbers, int index)
    {
        if (index >= 0 && index < phoneNumbers.Count)
        {
            return phoneNumbers[index];
        }
        return "";
    }
    private static void Main(string[] args)
    {
        string currentPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? "";
        var savePathExcel = currentPath.Split("bin")[0] + @"Excel File\";
        const string baseUrl = "https://www.khmer24.com/en/cars/all-cars.html?ad_condition=";

        // Create new instance
        // Tạo một instance mới
        var web = new HtmlWeb()
        {
            AutoDetectEncoding = false,
            OverrideEncoding = Encoding.UTF8
        };

        // List gender for product
        // List giới tính cho 2 site Đồng Hồ Nữ và Đồng Hồ Nam
        var types = new List<string>() { "new", "used" };

        // List product crawl
        // List lưu danh sách các sản phẩm Crawl được
        var listDataExport = new List<ProductModel>();

        Console.WriteLine("Please do not turn off the app while crawling!");

        // Loop for 2 gender 
        // Lặp List gender
        foreach (var type in types)
        {
            var requestUrl = baseUrl + type;

            // Load HTML to document from requestUrl
            // Load trang web, nạp html vào document từ requestUrl
            var documentForGetTotalPage = web.Load(requestUrl);

            var checkElements = documentForGetTotalPage
                .DocumentNode
                .QuerySelectorAll(".page-item > a")
                .Select(a => a.Attributes["href"].Value)
                .ToList()
                .Last();

            int totalProductsResult;
            string textTotalProductsResult = StringHelper.ProcessUrl(checkElements);
            //Console.WriteLine(">>>check totalProductsResult: {0}", textTotalProductsResult);
            int.TryParse(textTotalProductsResult, out totalProductsResult);

            for (int i = 0; i <= 50 /*totalProductsResult*/; i += 50)
            {
                string typeUrl = $"https://www.khmer24.com/en/cars/all-cars.html?ad_condition={type}&per_page={i}";
                //Console.WriteLine(">>>check typeUrl: {0}", typeUrl);
                var documentForGetLinkProducts = web.Load(typeUrl);
                var getLinkProdcuts = documentForGetLinkProducts
                    .DocumentNode
                    .QuerySelectorAll(".item > a")
                    .Select(a => a.Attributes["href"].Value)
                    .ToList();

                foreach (var linkProduct in getLinkProdcuts)
                {
                    string linkOneProduct = linkProduct.ToString();
                    //Console.WriteLine(">>> Check linkProduct: {0}", linkOneProduct);
                    var documentForGetProducts = web.Load(linkOneProduct);
                    var listNodeProductItem = documentForGetProducts
                        .DocumentNode
                        .QuerySelectorAll(".bg-white.border.rounded")
                        .ToList()
                        .Where(s => !string.IsNullOrEmpty(s.InnerText))
                        .ToList();

                    foreach (var product in listNodeProductItem) 
                    {
                        //Get product name
                        var productNameNode = product.QuerySelector("h1");
                        var productName = productNameNode != null ? productNameNode.InnerText.RemoveBreakLineTab() : "";
                        //Console.WriteLine("Check product Name: {0}", productName);

                        //Get product price
                        var productPriceNode = product.QuerySelector("b.price");
                        var productPrice = productPriceNode != null ? productPriceNode.InnerText.RemoveBreakLineTab() : "";
                        //Console.WriteLine("Check product Price: {0}", productPrice);


                        //Get product detail
                        var productDetails = product
                        .QuerySelectorAll(".item-detail > .list-unstyled > li > div > .value")
                        .ToList();

                        string carMakes = GetInnerTextSafely(productDetails, 0);
                        string carModel = GetInnerTextSafely(productDetails, 1);
                        string year = GetInnerTextSafely(productDetails, 2);
                        string taxType = GetInnerTextSafely(productDetails, 3);
                        string condition = GetInnerTextSafely(productDetails, 4);
                        string transmission = GetInnerTextSafely(productDetails, 5);
                        string color = GetInnerTextSafely(productDetails, 6);



                        //Get product detail ID
                        var productDetailId = product
                            .QuerySelectorAll(".item-header > .item-short-description > .list-unstyled > li > .value")
                            .ToList();
                        string id = GetInnerTextSafely(productDetailId, 0);
                        string category = GetInnerTextSafely(productDetailId, 1);
                        string locations = GetInnerTextSafely(productDetailId, 2);
                        string posted = GetInnerTextSafely(productDetailId, 3);


                        //Get product phone number
                        var productPhoneNumber = product
                            .QuerySelectorAll(".list_numbers > .list-unstyled > .number > a")
                            .Select(a => a.Attributes["href"].Value)
                            .ToList();

                        var phoneNumber = new List<string>();

                        foreach (var tel in productPhoneNumber) 
                        {
                            var getPhoneNumber = StringHelper.ProcessTel(tel);
                            phoneNumber.Add(getPhoneNumber);
                        }
                        //string phoneNumber0 = GetPhoneNumberSafely(phoneNumber, 0);
                        //string phoneNumber1 = GetPhoneNumberSafely(phoneNumber, 1);
                        //string phoneNumber2 = GetPhoneNumberSafely(phoneNumber, 2);
                            

                        listDataExport.Add(new ProductModel()
                        {
                            ProducOrder = (listDataExport.Count + 1).ToString(),
                            ID = id,
                            Category = category,
                            Locations = locations,
                            Posted = posted,
                            ProductName = productName,
                            Price = productPrice,
                            CarMakes = carMakes,
                            TaxType = taxType,
                            Color = color,
                            CarModel = carModel,
                            Condition = condition,
                            Year = year,
                            Transmission = transmission,
                            //PhoneNumber0 = GetPhoneNumberSafely(phoneNumber, 0),
                            //PhoneNumber1 = GetPhoneNumberSafely(phoneNumber, 1),
                            //PhoneNumber2 = GetPhoneNumberSafely(phoneNumber, 2),
                        });
                        //Console.WriteLine(">>> Check listDataExport.ID: {0}", listDataExport[0].ID);
                        //Console.WriteLine(">>> Check listDataExport.Category: {0}", listDataExport[0].Category);
                        //Console.WriteLine(">>> Check listDataExport.Locations: {0}", listDataExport[0].Locations);
                        //Console.WriteLine(">>> Check listDataExport.Posted: {0}", listDataExport[0].Posted);
                        //Console.WriteLine(">>> Check listDataExport.ProductName: {0}", listDataExport[0].ProductName);
                        //Console.WriteLine(">>> Check listDataExport.Price: {0}", listDataExport[0].Price);
                        //Console.WriteLine(">>> Check listDataExport.CarMakes: {0}", listDataExport[0].CarMakes);
                        //Console.WriteLine(">>> Check listDataExport.TaxType: {0}", listDataExport[0].TaxType);
                        //Console.WriteLine(">>> Check listDataExport.Color: {0}", listDataExport[0].Color);
                        //Console.WriteLine(">>> Check listDataExport.CarModel: {0}", listDataExport[0].CarModel);
                        //Console.WriteLine(">>> Check listDataExport.Condition: {0}", listDataExport[0].Condition);
                        //Console.WriteLine(">>> Check listDataExport.Year: {0}", listDataExport[0].Year);
                        //Console.WriteLine(">>> Check listDataExport.Transmission: {0}", listDataExport[0].Transmission);
                        //Console.WriteLine(">>> Check listDataExport.PhoneNumber0 {0}", listDataExport[0].PhoneNumber0);
                        //Console.WriteLine(">>> Check listDataExport.PhoneNumber1: {0}", listDataExport[0].PhoneNumber1);
                        //Console.WriteLine(">>> Check listDataExport.PhoneNumber2: {0}", listDataExport[0].PhoneNumber2);

                    }
                }
            }    
        }
        string csvFilePath = savePathExcel + DateTime.Now.Ticks+ "-car-khmer24h.CSV";
        ExportCSV.ExportToCsv(listDataExport, csvFilePath);
        var fileName = DateTime.Now.Ticks + "_Hayzzys-crawl.xlsx";
        // Export data to Excel
        ExportToExcel<ProductModel>.GenerateExcel(listDataExport, savePathExcel + fileName, "_hayzzys-crawl");

        Console.WriteLine("DONE !!!");
    }
}
