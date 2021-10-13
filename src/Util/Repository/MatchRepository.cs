using System.Threading.Tasks;
using Util.Model;
using Microsoft.Azure.Cosmos;

namespace Util.Repository
{
    public class MatchRepository
    {
        private readonly CosmosClient client;

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
            this.client = new CosmosClient(connectionString, clientOptions);
            
            this.container = client.GetContainer("pfmatchhistory-db", "match");
        }

        public Task<ItemResponse<Match>> Save(Match item)
        {
            return this.container.CreateItemAsync(item);
        }

        public Task<ItemResponse<Match>> Get(string id)
        {
            return this.container.ReadItemAsync<Match>(id, new PartitionKey(id));
        }
    }
}