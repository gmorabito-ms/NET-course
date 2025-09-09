namespace ApiEcommerce1.Mapping;

using ApiEcommerce1.Models;
using ApiEcommerce1.Models.Dtos;
using AutoMapper;

class ProductProfile:Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductDto>()
        .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
        .ReverseMap();
        CreateMap<Product, CreateProductDto>().ReverseMap();
        CreateMap<Product, UpdateProductDto>().ReverseMap();
    }
}