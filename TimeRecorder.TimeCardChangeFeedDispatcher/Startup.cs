using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using TimeRecorder.Core.Constants;

[assembly: FunctionsStartup(typeof(TimeRecorder.DailyRecordChangeFeedDispatcher.Startup))]

namespace TimeRecorder.DailyRecordChangeFeedDispatcher
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton(_ =>
            {
                var cosmosClient = new CosmosClientBuilder(Environment.GetEnvironmentVariable("CosmosDBConnection"))
                    .WithSerializerOptions(new CosmosSerializationOptions { PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase })
                    .Build();

                return cosmosClient.GetContainer(CosmosDbConstants.DatabaseName, CosmosDbConstants.MonthlyRecordContainerName);
            });
        }
    }
}
