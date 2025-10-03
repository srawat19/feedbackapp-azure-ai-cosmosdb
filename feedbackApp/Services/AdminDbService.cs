
using System.Net;
using feedbackApp.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace feedbackApp.Services
{

    public class AdminDbService : IDbService<AdminEvents>, IAdminDbService
    {
        private Container cosmosContainer;
        private readonly ILogger<AdminDbService> _logger;
        

        public AdminDbService(CosmosClient cosmosClient, IOptions<CosmosDbConfiguration> cosmosConfigOptions, ILogger<AdminDbService> logger)
        {
            _logger = logger;

            if (cosmosClient != null && cosmosConfigOptions != null && cosmosConfigOptions.Value != null)
            {
                CosmosDbConfiguration cosmosConfig = cosmosConfigOptions.Value;
                cosmosContainer = cosmosClient.GetContainer(cosmosConfig?.Database, cosmosConfig?.AdminContainer);

            }

        }

        public async Task<IEnumerable<AdminEvents>> FetchAsync(string emailAddress)
        {
            List<AdminEvents> adminEventsList = new List<AdminEvents>();
            try
            {
                string getRelatedAdminEvents = "select * from c where c.email=@email";
                QueryDefinition queryDef = new QueryDefinition(getRelatedAdminEvents).WithParameter("@email", emailAddress);
                QueryRequestOptions options = new() { PartitionKey = new PartitionKey(emailAddress) };
                //FeedIterator<AdminEvents> adminEventsFeed = cosmosContainer.GetItemQueryIterator<AdminEvents>(queryDef, null, options);
                FeedIterator<AdminEvents> adminEventsFeed = cosmosContainer.GetItemQueryIterator<AdminEvents>(queryDef);


                while (adminEventsFeed.HasMoreResults)
                {
                    var result = await adminEventsFeed.ReadNextAsync();
                    adminEventsList.AddRange(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred trying to fetch data from Cosmos DB - Admin Container.");

            }
            return adminEventsList;

        }

        public async Task<AdminEvents> FetchAsyncByEventShortName(string eventShortName)
        {
            List<AdminEvents> adminEventsList = new List<AdminEvents>();
            try
            {
                string getRelatedAdminEvents = "select * from c where c.eventShortName=@eventShortName";
                QueryDefinition queryDef = new QueryDefinition(getRelatedAdminEvents).WithParameter("@eventShortName", eventShortName);
                //QueryRequestOptions options = new() { PartitionKey = new PartitionKey(email) };
                FeedIterator<AdminEvents> eventFeed = cosmosContainer.GetItemQueryIterator<AdminEvents>(queryDef);

                while (eventFeed.HasMoreResults)
                {
                    var result = await eventFeed.ReadNextAsync();
                    return result.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred trying to fetch data from Cosmos DB based on EventShortName - Admin Container.");

            }
            return null;

        }

    public async Task<string> FetchAsyncByEventId(string id)
        {
            try
            {
                string getRelatedAdminEvents = "select * from c where c.eventId=@id";
                QueryDefinition queryDef = new QueryDefinition(getRelatedAdminEvents).WithParameter("@id", id);
                //QueryRequestOptions options = new() { PartitionKey = new PartitionKey(email) };
                FeedIterator<AdminEvents> eventFeed = cosmosContainer.GetItemQueryIterator<AdminEvents>(queryDef);

                while (eventFeed.HasMoreResults)
                {
                    var result = await eventFeed.ReadNextAsync();
                    return result.FirstOrDefault().EventName;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred trying to fetch data from Cosmos DB based on EventShortName - Admin Container.");

            }
            return null;

        }



        Task<HttpStatusCode> IDbService<AdminEvents>.SaveAsync(AdminEvents entity)
        {
            throw new NotImplementedException();
            //TBD : Form for an Admin user to enter event details.
            //eventshortname will be auto-generated.
        }
    }
}