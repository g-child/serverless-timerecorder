using Microsoft.Azure.WebJobs;
using TimeRecorder.Core.Models;

namespace TimeRecorder.TimeCardWriter
{
    public static class Function
    {
        [FunctionName("TimeCardWriter")]
        public static void Run(
            [QueueTrigger("timecard-writer")] TimeCard input,
            [CosmosDB(
                databaseName: "TimeRecorder",
                collectionName: "TimeCard",
                ConnectionStringSetting = "CosmosDBConnection")] out TimeCard output
            )
        {
            output = input;
        }
    }
}
