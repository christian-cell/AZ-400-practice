using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using User.Models.Commands;
using User.Models.Responses;
using User.Services.Abstractions;

namespace Users.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class UserController : Base
    {
        
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        public UserController(ILogger<UserController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }
        
        [HttpGet]
        public async Task<ActionResult<List<UserResponse>>> GetUsersAsync()
        {
            var sales = await _userService.GetUsersAsync();
            
            return Ok(sales);
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponse>> CreateUserAsync([FromBody] UserCommand command)
        {
            var response = await _userService.CreateUserAsync(command);
            
            return Ok(response);
        }

        [HttpPut]
        public async Task<ActionResult<BaseResponse>> UpdateUserAsync([FromBody] UserUpdateCommand command)
        {
            var response = await _userService.UpdateUserAsync(command);

            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult<BaseResponse>> DeleteUserAsync(Guid id)
        {
            var response = await _userService.DeleteUserAsync(id);
            return Ok(response);
        }
    }
};

