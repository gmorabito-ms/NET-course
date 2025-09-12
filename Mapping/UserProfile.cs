using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce1.Models;
using ApiEcommerce1.Models.Dtos;
using AutoMapper;

namespace ApiEcommerce1.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<User, CreateUserDto>().ReverseMap();
        CreateMap<User, UserLoginDto>().ReverseMap();
        CreateMap<User, UserLoginResponseDto>().ReverseMap();
        CreateMap<ApplicationUser, UserDataDto>().ReverseMap();
        CreateMap<ApplicationUser, UserDto>().ReverseMap();
    }
}