using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System;
using TimeRecorder.Core.Constants;
using TimeRecorder.Core.Models;

namespace TimeRecorder.DailyRecordCreater
{
    public static class ClockOutFunction
    {
        [FunctionName("ClockOut")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "dailyrecord/clockOut")] HttpRequest req,
            [Queue("timecard-writer", Connection = "TimeCardWriterQueueConnectionString")] out TimeCard output
            )
        {
            var jstNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time"));

            output = new TimeCard(jstNow, RecordType.ClockOut, "g-child");

            return new OkObjectResult($"{jstNow}");
        }
    }
}
