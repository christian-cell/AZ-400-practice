using Microsoft.AspNetCore.Mvc;

namespace UsersACA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class UserController : Base
    {
        [HttpGet]
        public string GreatPople()
        {
            
            return "Hola que asee";
        }
    }
};

