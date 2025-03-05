using Microsoft.AspNetCore.Mvc;
using UsersACA.Services.Abstractions;

namespace UsersACA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class UserController : Base
    {
        private readonly IUsersService _usersService;

        public UserController(IUsersService usersService)
        {
            _usersService = usersService;
        }


        [HttpGet]
        public string GreatPople()
        {

            return _usersService.GreatPeople();
        }
    }
};

