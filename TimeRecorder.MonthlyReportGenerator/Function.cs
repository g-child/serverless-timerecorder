using ClosedXML.Excel;
using Microsoft.Azure.WebJobs;
using System.IO;
using TimeRecorder.Core.Constants;
using TimeRecorder.Core.Models;

namespace TimeRecorder.MonthlyReportGenerator
{
    public static class Function
    {
        [FunctionName("Function")]
        public static void Run(
            [QueueTrigger("create-report")] ReportCreateMessage message,
            [Blob("template/template.xlsx", FileAccess.Read)] Stream input,
            [CosmosDB(
                databaseName: CosmosDbConstants.DatabaseName,
                collectionName: CosmosDbConstants.MonthlyRecordContainerName,
                ConnectionStringSetting = "CosmosDBConnection",
                Id = "{MonthlyRecordId}",
                PartitionKey = "{UserId}")] MonthlyRecord monthlyRecord,
            [Blob("monthlyreport/{TargetMonth.Year}-{TargetMonth.Month}-{UserId}.xlsx", FileAccess.Write)] Stream output
           )
        {
            using var stream = CreateMonthlyReport(input, message, monthlyRecord);
            stream.CopyTo(output);
        }

        private static Stream CreateMonthlyReport(Stream template, ReportCreateMessage message, MonthlyRecord monthlyRecord)
        {
            var report = new XLWorkbook(template);

            var sheet = report.Worksheet("template").CopyTo($"{message.TargetMonth.Year}{message.TargetMonth.Month}");
            sheet.Cell("C4").Value = message.TargetMonth.Year;
            sheet.Cell("C5").Value = message.TargetMonth.Month;
            sheet.Cell("C6").Value = message.UserId;

            foreach (var reportItem in monthlyRecord.Report)
            {
                var cellAddressOfStart = $"F{reportItem.Key + 5}";
                var cellAddressOfEnd = $"G{reportItem.Key + 5}";
                sheet.Cell(cellAddressOfStart).Value = reportItem.Value.Start;
                sheet.Cell(cellAddressOfEnd).Value = reportItem.Value.End;
            }

            return SaveAsStream(report);
        }

        private static Stream SaveAsStream(XLWorkbook workbook)
        {
            var memoryStream = new MemoryStream();
            workbook.SaveAs(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return memoryStream;
        }
    }
}
