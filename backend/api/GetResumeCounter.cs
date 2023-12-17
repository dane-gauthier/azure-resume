using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Company.Function
{
    public static class AzureResumeCounter
    {
        [FunctionName("AzureResumeCounter")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "AzureResume",
                containerName: "Counter",
                Connection = "AzureResumeConnectionString",
                Id = "1",
                PartitionKey = "1",
                CreateIfNotExists = true)] Counter counter,
            [CosmosDB(
                databaseName: "AzureResume",
                containerName: "Counter",
                Connection = "AzureResumeConnectionString",
                Id = "1",
                PartitionKey = "1")] IAsyncCollector<Counter> updatedCounter,
            ILogger log)
        {
            log.LogInformation($"C# HTTP trigger function processed a request for AzureResumeCounter.");

            try
            {
                // Check if the counter object is null
                if (counter == null)
                {
                    log.LogWarning("Counter object is null. Creating a new one.");

                    // Creating a new counter since it doesn't exist
                    counter = new Counter { Id = "1", Count = 0 };
                }

                // Increment the count
                counter.Count++;

                // Update the counter in Cosmos DB
                await updatedCounter.AddAsync(counter);

                var jsonToReturn = JsonConvert.SerializeObject(counter);

                log.LogInformation($"Retrieved Counter from Cosmos DB: {jsonToReturn}");

                return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent(jsonToReturn, Encoding.UTF8, "application/json")
                };
            }
            catch (Exception ex)
            {
                log.LogError($"An error occurred: {ex.Message}");
                return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
            }
        }
    }
}