using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker.Extensions.Sql;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using Newtonsoft.Json;

namespace My.Functions
{
    public class AddRow
    {
        private readonly ILogger<AddRow> _logger;

        public AddRow(ILogger<AddRow> logger)
        {
            _logger = logger;
        }

        //Set value
        [Function("AddRow")]
        public async Task<OutputType> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route="AddRow")] HttpRequestData req,
    FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("AddRow");
            logger.LogInformation("C# HTTP trigger function processed a request.");
            
            var message = await req.ReadAsStringAsync();
            //Deserialize
            leaderboardRow leaderboardInput = JsonConvert.DeserializeObject<leaderboardRow>(message);
            Console.WriteLine(leaderboardInput.player);
            Console.WriteLine(leaderboardInput.score);

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            await response.WriteStringAsync("Success");

            // Return a response to both HTTP trigger and Azure SQL output binding.
            return new OutputType()
            {
                ToDoItem = new ToDoItem
                {
                    Id = Guid.NewGuid(),
                    title = message,
                    completed = false,
                    url = ""
                },
                HttpResponse = response
            };
        }
    }
    //Get value
    public static class GetItems
    {
        [Function("GetItems")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetItems")]
            HttpRequest req,
            [SqlInput(commandText: "SELECT TOP (1000) * FROM [dbo].[ToDo]",
                commandType: System.Data.CommandType.Text,
                parameters: "",
                connectionStringSetting: "SqlConnectionString")]
            IEnumerable<ToDoItem> toDoItem)
        {
            return new OkObjectResult(toDoItem);
        }
    }

    //from tutorial
    public class ToDoItem
    {
        public Guid Id { get; set; }
        public int? order { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public bool? completed { get; set; }
    }

    //consistent
    public class OutputType
    {
        [SqlOutput("dbo.ToDo", connectionStringSetting: "SqlConnectionString")]
        public ToDoItem ToDoItem { get; set; }
        public HttpResponseData HttpResponse { get; set; }
    }

    //custom
    public class leaderboardRow
    {
        public string player { get; set; }
        public int score { get; set; }
    }
}



