using User.Models.Commands;
using User.Models.Responses;

namespace User.Services.Abstractions
{
    public interface IUserService
    {
        Task<List<UserResponse>> GetUsersAsync();
        Task<BaseResponse> CreateUserAsync(UserCommand command);
        Task<BaseResponse> UpdateUserAsync(UserUpdateCommand command);
        Task<BaseResponse> DeleteUserAsync(Guid id);
    }
};

