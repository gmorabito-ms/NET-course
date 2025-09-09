using ApiEcommerce1.Models.Dtos;
using AutoMapper;

namespace ApiEcommerce1.Mapping;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryDto>().ReverseMap();
        CreateMap<Category, CreateCategoryDto>().ReverseMap();
    }
}