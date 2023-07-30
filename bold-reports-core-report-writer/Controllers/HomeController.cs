using bold_reports_core_report_writer.Models;
using BoldReports.Writer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace bold_reports_core_report_writer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        // IWebHostEnvironment used with sample to get the application data from wwwroot.
        private Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostingEnvironment;

        // IWebHostEnvironment initialized with controller to get the data from application data folder.
        public HomeController(Microsoft.AspNetCore.Hosting.IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult Export(string writerFormat)
        {
            // Here, we have loaded the sales-order-detail sample report from application the folder wwwroot\Resources.
            FileStream inputStream = new FileStream(_hostingEnvironment.WebRootPath + @"\Resources\sales-order-detail.rdl", FileMode.Open, FileAccess.Read);
            MemoryStream reportStream = new MemoryStream();
            inputStream.CopyTo(reportStream);
            reportStream.Position = 0;
            inputStream.Close();
            ReportWriter writer = new ReportWriter();

            string fileName = null;
            WriterFormat format;
            string type = null;

            if (writerFormat == "PDF")
            {
                fileName = "sales-order-detail.pdf";
                type = "pdf";
                format = WriterFormat.PDF;
            }
            else if (writerFormat == "Word")
            {
                fileName = "sales-order-detail.docx";
                type = "docx";
                format = WriterFormat.Word;
            }
            else if (writerFormat == "CSV")
            {
                fileName = "sales-order-detail.csv";
                type = "csv";
                format = WriterFormat.CSV;
            }
            else
            {
                fileName = "sales-order-detail.xlsx";
                type = "xlsx";
                format = WriterFormat.Excel;
            }

            writer.LoadReport(reportStream);
            MemoryStream memoryStream = new MemoryStream();
            writer.Save(memoryStream, format);

            // Download the generated export document to the client side.
            memoryStream.Position = 0;
            FileStreamResult fileStreamResult = new FileStreamResult(memoryStream, "application/" + type);
            fileStreamResult.FileDownloadName = fileName;
            return fileStreamResult;
        }
    }
}
