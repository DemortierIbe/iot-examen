

namespace MCT.Function
{
    public static class parking
    {
        [FunctionName("addNummerplaat")]
        public static async Task<IActionResult> addNummerplaat(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/addnummerplaat")] HttpRequest req,
            ILogger log)
        {
            try
            {
                string json = await new StreamReader(req.Body).ReadToEndAsync();

                var nummerplaat = JsonConvert.DeserializeObject<Nummerplaat>(json);

                var connectionstring = Environment.GetEnvironmentVariable("CosmosDB");

                CosmosClientOptions options = new CosmosClientOptions()
                {
                    ConnectionMode = ConnectionMode.Gateway
                };
                CosmosClient client = new CosmosClient(connectionstring, options);
                var container = client.GetContainer("DemortierIbe", "parkeren");

                nummerplaat.Id = Guid.NewGuid().ToString();
                await container.CreateItemAsync(nummerplaat, new PartitionKey(nummerplaat.Id));


                return new OkObjectResult(nummerplaat);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [FunctionName("GetNummerplaat")]
        public static async Task<IActionResult> GetNummerplaat(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/getnummerplaat/{nummerplaat}")] HttpRequest req,
            ILogger log)
        {

            try
            {

                var connectionString = Environment.GetEnvironmentVariable("CosmosDb");

                CosmosClientOptions options = new CosmosClientOptions()
                {
                    ConnectionMode = ConnectionMode.Gateway
                };
                CosmosClient client = new CosmosClient(connectionString, options);
                var container = client.GetContainer("DemortierIbe", "nummerplaat");

                string sql = "SELECT * , sum(prijs) FROM c WHERE c.nummerplaat = '" + req.Query["nummerplaat"] + "'";
                var iterator = container.GetItemQueryIterator<Nummerplaat>(sql);
                var results = new List<Nummerplaat>();
                while (iterator.HasMoreResults)
                {
                    var response = await iterator.ReadNextAsync();
                    results.AddRange(response.ToList());
                }

                return new OkObjectResult(results);

            }
            catch (System.Exception ex)
            {
                log.LogError(ex.Message);
                return new BadRequestObjectResult(ex);
            }
        }

        [FunctionName("postPrijs")]
        public static async Task<IActionResult> postPrijs(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/postprijs/{id}")] HttpRequest req,
            ILogger log)
        {
            try
            {
                string json = await new StreamReader(req.Body).ReadToEndAsync();

                var nummerplaat = JsonConvert.DeserializeObject<Nummerplaat>(json);

                var connectionstring = Environment.GetEnvironmentVariable("CosmosDB");

                CosmosClientOptions options = new CosmosClientOptions()
                {
                    ConnectionMode = ConnectionMode.Gateway
                };
                CosmosClient client = new CosmosClient(connectionstring, options);
                var container = client.GetContainer("DemortierIbe", "parkeren");

                nummerplaat.Id = Guid.NewGuid().ToString();
                await container.CreateItemAsync(nummerplaat, new PartitionKey(nummerplaat.Id));



                return new OkObjectResult(nummerplaat);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                return new BadRequestObjectResult(ex.Message);
            }
        }

    }
}
