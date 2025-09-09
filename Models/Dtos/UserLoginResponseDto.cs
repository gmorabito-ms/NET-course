namespace ApiEcommerce.Models.Dtos;

public class UserLoginResponseDto
{
  public string? ID { get; set; }

  public UserRegisterDto? User { get; set; }
  public required string? Token { get; set; }
  public string? Message { get; set; }
}