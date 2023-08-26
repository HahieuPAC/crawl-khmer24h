using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using CsvHelper.Configuration;
using CrawlDataWebsiteTool.Models;

namespace CrawlDataWebsiteToolBasic.ExportTool
{
    internal class ExportCSV
    {
        public static void ExportToCsv(List<ProductModel> products, string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                TrimOptions = TrimOptions.Trim
            }))
            {
                csv.WriteRecords(products);
            }

            Console.WriteLine("CSV file exported successfully to: " + filePath);
        }
    }
}
