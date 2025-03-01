using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;

namespace User.Functions.Functions
{
    public class Users
    {
        /// <summary>
        /// greetings
        /// </summary>
        /// <param name="req"> The HTTP request data.</param>
        /// <returns> a warm greeting </return>
        [Function("greet-user")]
        [AllowAnonymous]
        [OpenApiOperation(operationId: "GreatUser", tags: new[] { "user" })]
        public string GreatUser([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req )
        {
            try
            {
                return "Helloo world !!";
            }
            catch(Exception ex)
            {
                throw new Exception($"something went wrong creating new user {ex}");
            }
            
        }
    }
};

