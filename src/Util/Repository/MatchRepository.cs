using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Threading.Tasks;
using Util.Model;

namespace Util.Repository
{
    public class MatchRepository
    {
        private readonly Container container;

        public MatchRepository(string connectionString)
        {
            var clientOptions = new CosmosClientOptions
            {
                SerializerOptions = new CosmosSerializationOptions
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                }
            };
            CosmosClient client = new CosmosClient(connectionString, clientOptions);

            this.container = client.GetContainer("pfmatchhistory-db", "match");
        }

        public Task<ItemResponse<Match>> Save(Match item)
        {
            return this.container.CreateItemAsync(item);
        }

        public async Task<List<Match>> Get(string id)
        {
            QueryDefinition query = new QueryDefinition("SELECT * FROM match c WHERE c.masterPlayerEntityId = @id").WithParameter("@id", id);
            List<Match> matches = new List<Match>();
            using (FeedIterator<Match> resultSetIterator = container.GetItemQueryIterator<Match>(
                query,
                requestOptions: new QueryRequestOptions()
                {
                    PartitionKey = new PartitionKey(id)
                }
                ))
            {
                while (resultSetIterator.HasMoreResults)
                {
                    FeedResponse<Match> response = await resultSetIterator.ReadNextAsync();
                    matches.AddRange(response);
                    // For Debug information: if(response.Diagnostics != null)
                }
            }

            return matches;
        }
    }
}