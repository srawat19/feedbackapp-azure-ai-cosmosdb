using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using feedbackApp.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace feedbackApp.Services
{
    public class CosmosDbService : IDbService<UserFeedback>
    {
        private Container cosmosContainer;
        private readonly ILogger<CosmosDbService> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cosmosClient"></param>
        /// <param name="cosmosConfigOptions"></param>
        public CosmosDbService(CosmosClient cosmosClient, IOptions<CosmosDbConfiguration> cosmosConfigOptions, ILogger<CosmosDbService> logger)
        {
            _logger = logger;
            
            if (cosmosClient != null && cosmosConfigOptions != null && cosmosConfigOptions.Value != null)
            {
                CosmosDbConfiguration cosmosConfig = cosmosConfigOptions.Value;
                cosmosContainer = cosmosClient.GetContainer(cosmosConfig?.Database, cosmosConfig?.Container);

            }


        }


        public async Task<HttpStatusCode> SaveAsync(UserFeedback feedbacks)
        {
            try
            {
                ItemResponse<UserFeedback> savedFeedback = await cosmosContainer.CreateItemAsync<UserFeedback>(feedbacks, new PartitionKey(feedbacks.EventId));
                return savedFeedback.StatusCode;
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex, $"Failed to Save into Cosmos DB with StatusCode {ex.StatusCode}");
                return ex.StatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to Save into Cosmos DB.");
                return HttpStatusCode.InternalServerError;
            }
            

        }



        public async Task<IEnumerable<UserFeedback>> FetchAsync(string eventId)
        {
            List<UserFeedback> userFeedbackList = new List<UserFeedback>();

            try
            {
                string selectQuery = "SELECT * FROM c";
                QueryDefinition queryDefinition = new QueryDefinition(selectQuery);

                FeedIterator<UserFeedback> eventFeeds = cosmosContainer.GetItemQueryIterator<UserFeedback>(queryDefinition, null, new QueryRequestOptions() { PartitionKey = new PartitionKey(eventId) });


                while (eventFeeds.HasMoreResults)
                {
                    var individualFeed = await eventFeeds.ReadNextAsync();

                    userFeedbackList.AddRange(individualFeed);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error occured trying to fetch data from Cosmos DB");

            }
                return userFeedbackList;
            
        }

        public async Task<double> GetEventsAverageScoreAsync(string eventId)
        {
            double avgRating = 0;

            try
            {
                var eventFeedbacks = await FetchAsync(eventId);
                if (eventFeedbacks != null && eventFeedbacks.Any())
                {
                    avgRating = eventFeedbacks.Average(e => e.Rating);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Failed computing event's average score.");
            }
            
            return avgRating;

        }
    }
    
}