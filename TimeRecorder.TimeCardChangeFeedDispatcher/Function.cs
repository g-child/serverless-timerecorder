using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeRecorder.Core.Constants;
using TimeRecorder.Core.Models;

namespace TimeRecorder.DailyRecordChangeFeedDispatcher
{
    public class Function
    {
        private readonly Container _container;

        public Function(Container container)
        {
            _container = container;
        }

        [FunctionName("Function")]
        public async Task RunAsync([CosmosDBTrigger(
            databaseName: CosmosDbConstants.DatabaseName,
            collectionName: CosmosDbConstants.TimeCardContainerName,
            LeaseCollectionName = CosmosDbConstants.TimeCardLeaseContainerName,
            ConnectionStringSetting = "CosmosDBConnection",
            CreateLeaseCollectionIfNotExists = true
            )] IReadOnlyList<Document> inputs
            )
        {
            foreach (var input in inputs)
            {
                var timeCard = JsonConvert.DeserializeObject<TimeCard>(input.ToString());
                var monthlyRecord = GetMonthlyRecordOrDefault(timeCard);

                if (monthlyRecord.Report.Any(r => r.Key == timeCard.RecordDate.Day))
                    monthlyRecord.UpdateRecord(timeCard);
                else
                    monthlyRecord.Report.Add(timeCard.RecordDate.Day, new MonthlyReportItem(timeCard));

                await _container.UpsertItemAsync(monthlyRecord, new Microsoft.Azure.Cosmos.PartitionKey(monthlyRecord.UserId));
            }
        }

        public MonthlyRecord GetMonthlyRecordOrDefault(TimeCard timeCard)
        {
            var targetMonth = new DateTime(timeCard.RecordDate.Year, timeCard.RecordDate.Month, 1);
            return _container.GetItemLinqQueryable<MonthlyRecord>(allowSynchronousQueryExecution: true, requestOptions: new QueryRequestOptions { PartitionKey = new Microsoft.Azure.Cosmos.PartitionKey(timeCard.UserId) })
                                        .AsEnumerable()
                                        .FirstOrDefault(r => r.ReportMonth == targetMonth) ?? new MonthlyRecord(targetMonth, timeCard.UserId);
        }
    }
}
