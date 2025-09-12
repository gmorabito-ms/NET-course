using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce1.Models;
using ApiEcommerce1.Models.Dtos;

namespace ApiEcommerce1.Repository.IRepository;

public interface IUserRepository
{
    // ICollection<User> GetUsers();
    ICollection<ApplicationUser> GetUsers();
    ApplicationUser? GetUser(string id);
    bool isUniqueUser(string username);

    Task<UserLoginResponseDto> Login(UserLoginDto userLoginDto);
    // Task<User> Register(CreateUserDto createUserDto);
    Task<UserDataDto> Register(CreateUserDto createUserDto);

}