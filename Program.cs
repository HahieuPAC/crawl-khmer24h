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
using System.IO;
using System.Collections.Generic;
using static System.Net.WebRequestMethods;
using MySqlConnector;
using System;

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
            return nodeList[index].InnerText;
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
        string connectionString = "Server=localhost;Port=4306;Database=khmer24h;User Id=root;Password=123456;";
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
        var listLinkProduct = new List<string>();
        var newLinkProduct = new HashSet<string>();
        var linkProductSet = new HashSet<string>(System.IO.File.ReadAllLines(currentPath.Split("bin")[0] + "saved-products.txt"));

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

            for (int i = 0; i < 50; i += 50)
            {
                string typeUrl = $"https://www.khmer24.com/en/cars/all-cars.html?ad_condition={type}&per_page={i}";
                //Console.WriteLine(">>>check typeUrl: {0}", typeUrl);
                var documentForGetLinkProducts = web.Load(typeUrl);
                var getLinkProdcuts = documentForGetLinkProducts
                    .DocumentNode
                    .QuerySelectorAll(".item > a")
                    .Select(a => a.Attributes["href"].Value)
                    .ToList();
                foreach (string linkProduct in getLinkProdcuts)
                {
                    if (!linkProductSet.Contains(linkProduct))
                    {
                        listLinkProduct.Add(linkProduct);
                        newLinkProduct.Add(linkProduct);
                    }
                }

            }
        }
        

        foreach (var linkProduct in newLinkProduct)
        {
            string linkOneProduct = linkProduct.ToString();
            var documentForGetProducts = web.Load(linkOneProduct);
            var firstNodeProductItem = documentForGetProducts
                .DocumentNode
                .QuerySelectorAll(".bg-white.border.rounded")
                .FirstOrDefault();

            if (firstNodeProductItem != null && !string.IsNullOrEmpty(firstNodeProductItem.InnerText))
            {
                //Get product name
                var productNameNode = firstNodeProductItem.QuerySelector("h1");
                var productName = productNameNode != null ? productNameNode.InnerText.RemoveBreakLineTab() : "";
                Console.WriteLine("Check product Name: {0}", productName);

                //Get product price
                var productPriceNode = firstNodeProductItem.QuerySelector("b.price");
                var productPrice = productPriceNode != null ? productPriceNode.InnerText.RemoveBreakLineTab() : "";
                Console.WriteLine("Check product Price: {0}", productPrice);

                //Get product detail
                var productDetails = firstNodeProductItem
                    .QuerySelectorAll(".item-detail > .list-unstyled > li > div > .value")
                    .ToList();
                string carMakes = GetInnerTextSafely(productDetails, 0);
                string carModel = GetInnerTextSafely(productDetails, 1);
                string year = GetInnerTextSafely(productDetails, 2);
                string taxType = GetInnerTextSafely(productDetails, 3);
                string productCondition = GetInnerTextSafely(productDetails, 4);
                string transmission = GetInnerTextSafely(productDetails, 5);
                string color = GetInnerTextSafely(productDetails, 6);

                //Get product detail ID
                var productDetailId = firstNodeProductItem
                    .QuerySelectorAll(".item-header > .item-short-description > .list-unstyled > li > .value")
                    .ToList();
                string id = GetInnerTextSafely(productDetailId, 0);
                string category = GetInnerTextSafely(productDetailId, 1);
                string locations = GetInnerTextSafely(productDetailId, 2);
                string posted = GetInnerTextSafely(productDetailId, 3);

                //Get product phone number
                var productPhoneNumber = firstNodeProductItem
                    .QuerySelectorAll(".list_numbers > .list-unstyled > .number > a")
                    .Select(a => a.Attributes["href"].Value)
                    .ToList();

                var phoneNumber = new List<string>();

                foreach (var tel in productPhoneNumber)
                {
                    var getPhoneNumber = StringHelper.ProcessTel(tel);
                    phoneNumber.Add(getPhoneNumber);
                }

                string phoneNumber0 = GetPhoneNumberSafely(phoneNumber, 0);
                string phoneNumber1 = GetPhoneNumberSafely(phoneNumber, 1);
                string phoneNumber2 = GetPhoneNumberSafely(phoneNumber, 2);

                string allPhoneNumber = "";

                foreach (var tel in phoneNumber)
                {
                    allPhoneNumber = allPhoneNumber + tel + ", ";
                }    

                //string phoneNumber0 = GetPhoneNumberSafely(phoneNumber, 0);
                //string phoneNumber1 = GetPhoneNumberSafely(phoneNumber, 1);
                //string phoneNumber2 = GetPhoneNumberSafely(phoneNumber, 2);
                Console.WriteLine(">>> Check phone number : {0}", allPhoneNumber);


                listDataExport.Add(new ProductModel()
                {
                    Link = linkOneProduct,
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
                    ProductCondition = productCondition,
                    Year = year,
                    Transmission = transmission,
                    PhoneNumber0 = phoneNumber0,
                    PhoneNumber1 = phoneNumber1,
                    PhoneNumber2 = phoneNumber2,
                });
            }
        }
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            foreach (var product in listDataExport)
            {
                var sql = @"INSERT INTO Product (Link, ID, Category, Locations, Posted, ProductName, Price, CarMakes, TaxType, Color, CarModel, ProductCondition, Year, Transmission, PhoneNumber1, PhoneNumber2, PhoneNumber3)
                    VALUES (@Link, @ID, @Category, @Locations, @Posted, @ProductName, @Price, @CarMakes, @TaxType, @Color, @CarModel, @ProductCondition, @Year, @Transmission, @PhoneNumber1, @PhoneNumber2, @PhoneNumber3)";

                using (var cmd = new MySqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@Link", product.Link);
                    cmd.Parameters.AddWithValue("@ID", product.ID);
                    cmd.Parameters.AddWithValue("@Category", product.Category);
                    cmd.Parameters.AddWithValue("@Locations", product.Locations);
                    cmd.Parameters.AddWithValue("@Posted", product.Posted);
                    cmd.Parameters.AddWithValue("@ProductName", product.ProductName);
                    cmd.Parameters.AddWithValue("@Price", product.Price);
                    cmd.Parameters.AddWithValue("@CarMakes", product.CarMakes);
                    cmd.Parameters.AddWithValue("@TaxType", product.TaxType);
                    cmd.Parameters.AddWithValue("@Color", product.Color);
                    cmd.Parameters.AddWithValue("@CarModel", product.CarModel);
                    cmd.Parameters.AddWithValue("@ProductCondition", product.ProductCondition);
                    cmd.Parameters.AddWithValue("@Year", product.Year);
                    cmd.Parameters.AddWithValue("@Transmission", product.Transmission);
                    cmd.Parameters.AddWithValue("@PhoneNumber1", product.PhoneNumber0);
                    cmd.Parameters.AddWithValue("@PhoneNumber2", product.PhoneNumber1);
                    cmd.Parameters.AddWithValue("@PhoneNumber3", product.PhoneNumber2);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        System.IO.File.AppendAllLines(currentPath.Split("bin")[0] + "saved-products.txt", newLinkProduct);
        var fileName = DateTime.Now.Ticks + "_Khmer24h_car.xlsx";
        // Export data to Excel
        ExportToExcel<ProductModel>.GenerateExcel(listDataExport, savePathExcel + fileName, "_hayzzys-crawl");

        Console.WriteLine("DONE !!!");
    }
}