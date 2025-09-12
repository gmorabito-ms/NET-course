using ApiEcommerce1.Constants;
using ApiEcommerce1.Models.Dtos;
using ApiEcommerce1.Repository.IRepository;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce1.Controllers.V2;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/v/{version:apiVersion}/[controller]")]
[ApiVersion("2.0")]
[Authorize]
// [EnableCors(PolicyNames.AllowSpecificOrigin)] // for all the methods inside this class
public class CategoriesController : ControllerBase
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CategoriesController(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    [AllowAnonymous] // allows access without auth
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    // [MapToApiVersion("2.0")]
    // [EnableCors(PolicyNames.AllowSpecificOrigin)] cors for an specifica method
    public IActionResult GetCategoriesOrderedById()
    {
        var categories = _categoryRepository.GetCategories().OrderBy(cat => cat.Id);
        var categoriesDto = new List<CategoryDto>();
        foreach (var category in categories)
        {
            categoriesDto.Add(_mapper.Map<CategoryDto>(category));
        }
        return Ok(categoriesDto);
    }


    [AllowAnonymous]
    [HttpGet("{id:int}", Name = "GetCategory")]
    // [ResponseCache(Duration = 10)]
    [ResponseCache(CacheProfileName = CacheProfiles.Default10)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetCategory(int id)
    {
        Console.WriteLine($"Category with id {id} at {DateTime.Now}");
        var category = _categoryRepository.GetCategory(id);
        Console.WriteLine($"Response with id {id}");
        if (category == null)
        {
            return NotFound($"Category with id: {id} not found");
        }
        var categoryDto = _mapper.Map<CategoryDto>(category);
        return Ok(categoryDto);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
    {
        if (createCategoryDto == null)
        {
            return BadRequest(ModelState);
        }
        if (_categoryRepository.CategoryExists(createCategoryDto.Name))
        {
            ModelState.AddModelError("CustomError", "Category already exists");
            return BadRequest(ModelState);
        }
        var category = _mapper.Map<Category>(createCategoryDto);
        if (!_categoryRepository.CreateCategory(category))
        {
            ModelState.AddModelError("CustomError", $"Something went wrong while saving the registry {category.Name}");
            return StatusCode(500, ModelState);
        }
        return CreatedAtRoute("GetCategory", new { id = category.Id }, category);
    }

    [HttpPatch("{id:int}", Name = "UpdateCategory")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult UpdateCategory(int id, [FromBody] CreateCategoryDto updateCategoryDto)
    {
        if (!_categoryRepository.CategoryExists(id))
        {
            return NotFound($"Category with id: {id} not found");
        }
        if (updateCategoryDto == null)
        {
            return BadRequest(ModelState);
        }
        if (_categoryRepository.CategoryExists(updateCategoryDto.Name))
        {
            ModelState.AddModelError("CustomError", "Category already exists");
            return BadRequest(ModelState);
        }
        var category = _mapper.Map<Category>(updateCategoryDto);
        category.Id = id;
        if (!_categoryRepository.UpdateCategory(category))
        {
            ModelState.AddModelError("CustomError", $"Something went wrong while updating the registry {category.Name}");
            return StatusCode(500, ModelState);
        }
        return NoContent();
    }


    [HttpDelete("{id:int}", Name = "DeleteCategory")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult DeleteCategory(int id)
    {
        if (!_categoryRepository.CategoryExists(id))
        {
            return NotFound($"Category with id: {id} not found");
        }
        var category = _categoryRepository.GetCategory(id);
        if (category == null)
        {
            return NotFound($"Category with id: {id} not found");
        }
        if (!_categoryRepository.DeleteCategory(category))
        {
            ModelState.AddModelError("CustomError", $"Something went wrong while deleting the registry {category.Name}");
            return StatusCode(500, ModelState);
        }
        return NoContent();
    }
}
