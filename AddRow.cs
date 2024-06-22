using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker.Extensions.Sql;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace My.Functions
{
    public class AddRow
    {
        private readonly ILogger<AddRow> _logger;

        public AddRow(ILogger<AddRow> logger)
        {
            _logger = logger;
        }

        [Function("AddRow")]
        public async Task<OutputType> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
    FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("AddRow");
            logger.LogInformation("C# HTTP trigger function processed a request.");
            
            var message = "Welcome to Azure Functions!";

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            await response.WriteStringAsync(message);

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
}

public class ToDoItem
{
    public Guid Id { get; set; }
    public int? order { get; set; }
    public string title { get; set; }
    public string url { get; set; }
    public bool? completed { get; set; }
}

public class OutputType
{
    [SqlOutput("dbo.ToDo", connectionStringSetting: "SqlConnectionString")]
    public ToDoItem ToDoItem { get; set; }
    public HttpResponseData HttpResponse { get; set; }
}