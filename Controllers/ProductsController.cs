using ApiEcommerce1.Models;
using ApiEcommerce1.Models.Dtos;
using ApiEcommerce1.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public ProductsController(IProductRepository productRepository, IMapper mapper, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetProducts()
    {
        var products = _productRepository.GetProducts();
        var productsDto = _mapper.Map<List<ProductDto>>(products); // instead of a foreach

        return Ok(productsDto);
    }

    [HttpGet("{productId:int}", Name = "GetProduct")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetProduct(int productId)
    {
        var product = _productRepository.GetProduct(productId);
        if (product == null)
        {
            return NotFound($"Product with id: {productId} not found");
        }
        var productDto = _mapper.Map<ProductDto>(product);
        return Ok(productDto);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult CreateProduct([FromBody] CreateProductDto createProductDto)
    {
        if (createProductDto == null)
        {
            return BadRequest(ModelState);
        }
        if (!_categoryRepository.CategoryExists(createProductDto.CategoryId))
        {
            ModelState.AddModelError("CustomError", $"Category with {createProductDto.CategoryId} do not exists");
            return BadRequest(ModelState);
        }
        var product = _mapper.Map<Product>(createProductDto);
        if (!_productRepository.CreateProduct(product))
        {
            ModelState.AddModelError("CustomError", $"Something went wrong while saving the registry {product.Name}");
            return StatusCode(500, ModelState);
        }
        var createdProduct = _productRepository.GetProduct(product.ProductId);
        var productdto = _mapper.Map<ProductDto>(createdProduct);
        return CreatedAtRoute("GetProduct", new { productId = product.ProductId }, productdto);
    }

    [HttpGet("searchProductByCategory/{categoryId:int}", Name = "GetProductsForCategory")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetProductsForCategory(int categoryId)
    {
        var products = _productRepository.GetProductsForCategory(categoryId);
        if (products.Count == 0)
        {
            return NotFound($"Products with categoryId: {categoryId} not found");
        }
        var productsDto = _mapper.Map<List<ProductDto>>(products);
        return Ok(productsDto);
    }

    // when we receive strings at params, we dont need to put the type string
    [HttpGet("searchProductByNameDescription/{searchTerm}", Name = "SearchProducts")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult SearchProducts(string searchTerm)
    {
        var products = _productRepository.SearchProducts(searchTerm);
        if (products.Count == 0)
        {
            return NotFound($"Products with name or description: '{searchTerm}' not found");
        }
        var productsDto = _mapper.Map<List<ProductDto>>(products);
        return Ok(productsDto);
    }

    [HttpPatch("buyProduct/{name}/{quantity:int}", Name = "BuyProduct")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult BuyProduct(string name, int quantity)
    {
        if (string.IsNullOrWhiteSpace(name) || quantity <= 0)
        {
            return BadRequest("Invalid product name or quantity");
        }
        var foundProduct = _productRepository.ProductExists(name);
        if (!foundProduct)
        {
            return BadRequest($"Product not found :{name}");
        }
        if (!_productRepository.BuyProduct(name, quantity))
        {
            ModelState.AddModelError("CustomError", $"Error buying the product {name} or quantity requested: {quantity} is unavailable");
            return BadRequest(ModelState);
        }
        return Ok($"Product {name} bought: {quantity}");
    }

    [HttpPut("{productId:int}", Name = "UpdateProduct")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult UpdateProduct(int productId, [FromBody] UpdateProductDto updateProductDto)
    {
        if (updateProductDto == null)
        {
            return BadRequest(ModelState);
        }
        if (!_productRepository.ProductExists(productId))
        {
            ModelState.AddModelError("CustomError", $"Product with {productId} do not exists");
            return BadRequest(ModelState);
        }
        if (!_categoryRepository.CategoryExists(updateProductDto.CategoryId))
        {
            ModelState.AddModelError("CustomError", $"Category with {updateProductDto.CategoryId} do not exists");
            return BadRequest(ModelState);
        }
        var product = _mapper.Map<Product>(updateProductDto);
        product.ProductId = productId;
        if (!_productRepository.UpdateProduct(product))
        {
            ModelState.AddModelError("CustomError", $"Something went wrong while updating the registry {product.Name}");
            return StatusCode(500, ModelState);
        }
        return NoContent();
    }

    [HttpDelete("{productId:int}", Name = "DeleteProduct")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult DeleteProduct(int productId)
    {
        if (productId == 0)
        {
            return BadRequest(ModelState);
        }
        var product = _productRepository.GetProduct(productId);
        if (product == null)
        {
            return NotFound($"Product with id: {productId} not found");
        }
        if (!_productRepository.DeleteProduct(product))
        {
            ModelState.AddModelError("CustomError", $"Something went wrong while deleting the registry {product.Name}");
            return StatusCode(500, ModelState);
        }
        return NoContent();
    }
}