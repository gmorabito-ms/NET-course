using ApiEcommerce.Models.Dtos;
using ApiEcommerce1.Models;

namespace ApiEcommerce1.Repository.IRepository;

public interface IUserRepository
{
    ICollection<User> GetUsers();
    User? GetUser(int id);
    bool isUniqueUser(string username);

    Task<UserLoginResponseDto> Login(UserLoginDto userLoginDto);
    Task<User> Register(CreateUserDto createUserDto);

}