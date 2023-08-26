using CrawlDataWebsiteTool.Models;
using CrawlDataWebsiteToolBasic.Functions;
using CrawlDataWebsiteToolBasic.Helpers;
using Fizzler.Systems.HtmlAgilityPack;
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

        // Spinner
        var spinner = new ConsoleSpinner();

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
            Console.WriteLine(">>>check totalProductsResult: {0}", textTotalProductsResult);
            int.TryParse(textTotalProductsResult, out totalProductsResult);

            for (int i = 0; i <= totalProductsResult; i += 50)
            {
                string typeUrl = $"https://www.khmer24.com/en/cars/all-cars.html?ad_condition={type}&per_page={i}";
                Console.WriteLine(">>>check typeUrl: {0}", typeUrl);
                var documentForGetLinkProducts = web.Load(typeUrl);
                var getLinkProdcuts = documentForGetLinkProducts
                    .DocumentNode
                    .QuerySelectorAll(".item > a")
                    .Select(a => a.Attributes["href"].Value)
                    .ToList();

                foreach (var linkProduct in getLinkProdcuts)
                {
                    string linkOneProduct = linkProduct.ToString();
                    Console.WriteLine(">>> Check linkProduct: {0}", linkOneProduct);
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
                        var productName = product
                            .QuerySelector("h1")
                            .InnerText
                            .RemoveBreakLineTab();
                        Console.WriteLine("Check product Name: {0}", productName);

                        //Get product price
                        var productPrice = product
                            .QuerySelector("b.price")
                            .InnerText
                            .RemoveBreakLineTab();
                        Console.WriteLine("Check product Price: {0}", productPrice);

                        //Get product detail
                        var productDetails = product
                            .QuerySelectorAll(".item-detail > .list-unstyled > li > div > .value")
                            .ToList();
                        string carMakes = productDetails[0].InnerText;
                        string carModel = productDetails[1].InnerText;
                        string year = productDetails[2].InnerText;
                        string TaxType = productDetails[3].InnerText;
                        string condition = productDetails[4].InnerText;
                        string Transmission = productDetails[5].InnerText;
                        string Color = productDetails[6].InnerText;



                        //Get product detail ID
                        var productDetailId = product
                            .QuerySelectorAll(".item-header > .item-short-description > .list-unstyled > li > .value")
                            .ToList();
                        string id = productDetailId[0].InnerText;
                        string category = productDetailId[1].InnerText;
                        string Locations = productDetailId[2].InnerText;
                        string Posted = productDetailId[3].InnerText;


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
                            
                        Console.WriteLine("check product phone number: {0}", phoneNumber[1]);

                        listDataExport.Add(new ProductModel()
                        {
                            //productname = productname,
                            //gender = gender.equals("nam") ? "nam" : "nữ",
                            //currency = "vnd",
                            //discountprice = discountprice,
                            //orginprice = orginprice,
                            //linkdetail = linkdetail,
                            //author = "https://www.code-mega.com"
                        });

                    }

                }
            }    
        }
    }
}

// Get total page from requestUrl
// Lấy tổng số trang từ requestUrl
//var texttotalpage = documentforgettotalpagemale
//    .documentnode
//    .queryselectorall(".page-numbers > li")
//    .tolist()
//    .where(s => !string.isnullorempty(s.innertext))
//    .tolist()
//    .last()
//    .innertext
//    .removebreaklinetab();

//    int totalPage;
//    int.TryParse(textTotalPage, out totalPage);

//    // Loop each page
//    // Lặp qua từng trang
//    for (var i = 1; i <= totalPage; i++)
//    {
//        var requestPerPage = baseUrl + $"/kieu-dang/{gender}/page/{i}";

//        // Load HTML to document from requestPerPage
//        // Load trang web, nạp html vào document từ requestPerPage
//        var documentForListItem = web.Load(requestPerPage);

//        // Get all Node Product
//        // Lấy tất cả các Node chứa thông tin của sản phẩm
//        var listNodeProductItem = documentForListItem
//            .DocumentNode
//            .QuerySelectorAll("div.shop-container " +
//                              "> div.products " +
//                              "> div.product-small " +
//                              "> div.col-inner " +
//                              "> div.product-small " +
//                              "> div.box-text")
//            .ToList();

//        // Loop for each Node
//        // Lặp qua các Node
//        foreach (var node in listNodeProductItem)
//        {
//            // Get Link Detail of Product
//            // Lấy Link chi tiết của sản phẩm
//            var linkDetail = node
//                .QuerySelector("div.title-wrapper > p.name > a")
//                .Attributes["href"]
//                .Value
//                .RemoveBreakLineTab();

//            // Get Product Name
//            // Lấy tên của sản phẩm
//            var productName = node
//                .QuerySelector("div.title-wrapper")
//                .InnerText
//                .RemoveBreakLineTab();

//            // Get Product Price includes Orgin Price and Discount Price
//            // Lấy giá của sản phẩm bao gồm giá gốc và giá khuyển mãi
//            var priceText = node
//                .QuerySelector("div.price-wrapper > span.price")
//                .InnerText
//                .RemoveBreakLineTab()
//                .ReplaceMultiToEmpty(new List<string>() { "₫", ",", "&#8363;" });

//            var priceList = priceText?.Split(" ").ToList();

//            // Parse text to price and format
//            var orginPrice = priceList?.Count > 0 ? double.Parse(priceList[0]).ToString("N2") : 0.ToString("N2");
//            var discountPrice = priceList?.Count > 0 ? double.Parse(priceList[1]).ToString("N2") : 0.ToString("N2");

//            // Add Product to listDataExport
//            // Thêm sản phẩm vào listDataExport
//            listDataExport.Add(new ProductModel()
//            {
//                ProductName = productName,
//                Gender = gender.Equals("nam") ? "Nam" : "Nữ",
//                Currency = "VND",
//                DiscountPrice = discountPrice,
//                OrginPrice = orginPrice,
//                LinkDetail = linkDetail,
//                Author = "https://www.code-mega.com"
//            });

//            // Print Spinner
//            spinner.Turn(displayMsg: $"Crawling {listDataExport.Count} Item(s) | ", sequenceCode: 5);
//        }
//    }
//}

//var fileName = DateTime.Now.Ticks + "_code-mega.xlsx";

//// Export data to Excel
//ExportToExcel<ProductModel>.GenerateExcel(listDataExport, savePathExcel + fileName, "Code Mega");

//Console.WriteLine($"Crawled {listDataExport.Count} Item(s)");
//Console.WriteLine($"Link saved file excel: " + savePathExcel + fileName);
//Console.WriteLine("DONE !!!");
//Console.WriteLine("Code Mega");