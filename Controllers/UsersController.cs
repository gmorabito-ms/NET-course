using ApiEcommerce.Models.Dtos;
using ApiEcommerce1.Models.Dtos;
using ApiEcommerce1.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce1.Controllers;

[ApiController]
[Route("api/[controller]")]

public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UsersController(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetUsers()
    {
        var users = _userRepository.GetUsers();
        var usersDto = _mapper.Map<List<UserDto>>(users); // instead of a foreach

        return Ok(usersDto);
    }

    [HttpGet("id:int", Name = "GetUser")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetUser(int id)
    {
        var user = _userRepository.GetUser(id);
        if (user == null)
        {
            return NotFound($"User with id: {id} not found");
        }
        var userDto = _mapper.Map<UserDto>(user); // instead of a foreach

        return Ok(userDto);
    }

    [HttpPost(Name = "RegisterUser")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RegisterUser([FromBody] CreateUserDto createUserDto)
    {
        if (createUserDto == null || !ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        if (string.IsNullOrWhiteSpace(createUserDto.Username))
        {
            return BadRequest("Username is required");
        }
        if (!_userRepository.isUniqueUser(createUserDto.Username))
        {
            return BadRequest("User already exists");
        }
        var result = await _userRepository.Register(createUserDto);
        if (result == null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "error creating user");
        }
        return CreatedAtRoute("GetUser", new { id = result.Id }, result);
    }


    [HttpPost("Login", Name = "LoginUser")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> LoginUser([FromBody] UserLoginDto userLoginDto)
    {
        if (userLoginDto == null || !ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userRepository.Login(userLoginDto);
        if (user == null)
        {
            return Unauthorized();
        }
        return Ok(user);
    }
}