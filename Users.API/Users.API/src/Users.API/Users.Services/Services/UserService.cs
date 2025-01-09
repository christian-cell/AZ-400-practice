using AutoMapper;
using Microsoft.Extensions.Logging;
using User.Domain.Infraestructure;
using User.Models.Commands;
using User.Models.Exceptions;
using User.Models.Responses;
using User.Repository;
using User.Services.Abstractions;

namespace User.Services.Services
{
    public class UserService: EntityRepository, IUserService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService( 
            
            UserDbContext userDbContext, 
            IMapper mapper, 
            ILogger<UserService> logger
            ) : base(userDbContext)
        {
            _mapper = mapper;
            _logger = logger;
        }
        
        public async Task<List<UserResponse>> GetUsersAsync()
        {
            var usersQueryable = await SearchAsync<Domain.Entities.User>(user => !string.IsNullOrEmpty(user.FirstName) );

            var users = _mapper.Map<List<UserResponse>>(usersQueryable);

            return users;
        }

        public async Task<BaseResponse> CreateUserAsync(UserCommand command)
        {
            try
            {
                var user = _mapper.Map<Domain.Entities.User>(command);

                _logger.LogInformation($"User {command.FirstName} created");

                await AddAsync<Domain.Entities.User>(user);
                
                await SaveAsync();

                return new BaseResponse()
                {
                    IsSuccess = true,
                    Messages = new List<Message>()
                    {
                        new Message("User created successfully", Message.MessageLevel.Info)
                    },
                    ItemId = user.Id
                };
            }
            catch (Exception e)
            {
                _logger.LogError($"something went wrong creating user with name {command.FirstName} , error : {e}");
                
                throw new Exception($"something went wrong creating user with name {command.FirstName} , error : {e}");
            }
        }

        public async Task<BaseResponse> UpdateUserAsync(UserUpdateCommand command)
        {
            try
            {
                var user = await FindAsync<Domain.Entities.User>(command.Id);

                if (user is null) throw new NotFoundException("user", "id", command.Id);

                if (user is null) throw new NotFoundException("payment", "id", command.Id);

                var newUser = _mapper.Map<Domain.Entities.User>(command);

                await UpdateAsync<Domain.Entities.User>(user.Id, newUser);
                await SaveAsync();

                return new BaseResponse
                {
                    IsSuccess = true,
                    Messages = new List<Message> { new Message($"user with id : '{newUser.Id}' was updated") },
                    ItemId = newUser.Id
                };
            }
            catch (Exception e)
            {
                _logger.LogError($"something went wrong updating user with id {command.Id} , error : {e}");
                
                throw new Exception($"something went wrong updating user with id {command.Id} , error : {e}");
            }
        }

        public async Task<BaseResponse> DeleteUserAsync(Guid id)
        {
            _logger.LogInformation($"...deleting user with id : '{id}'");
            
            if (id == null)throw new Exception($"user Guid is required and must be a valid Guid");
            
            var user = await FindAsync<Domain.Entities.User>(id);

            if(user is null) throw new NotFoundException("user" , "id" , id);
            
            bool ok = await DeleteAsync<Domain.Entities.User>(id).ConfigureAwait(false);
            if (ok) ok = await SaveAsync().ConfigureAwait(false);

            if (!ok)
            {
                throw new Exception($"Failed to delete user with id : '{id}'");
            }

            _logger.LogInformation($"User removed: '{id}'");
            
            return new BaseResponse
            {
                IsSuccess = true,
                Messages = new List<Message> { new Message($"user with id : '{user.Id}' was deleted") },
                ItemId = user.Id
            };
        }
    }
};

