using Microsoft.Azure.WebJobs;
using System;
using System.Threading.Tasks;
using TimeRecorder.Core.Models;

namespace TimeRecorder.MonthStartPublisher
{
    public static class Function
    {
        [FunctionName("Function")]
        public static async Task Run(
            [TimerTrigger("0 0 9 1 * *")] TimerInfo myTimer,
            [Queue("create-report", Connection = "ReportCreatorStorageConnectionString")] IAsyncCollector<ReportCreateMessage> outputs
            )
        {
            var now = DateTime.UtcNow;

            await outputs.AddAsync(
                new ReportCreateMessage { UserId = "g-child", TargetMonth = new DateTime(now.Year, now.Month, 1).AddMonths(-1) }
            );
        }
    }
}
