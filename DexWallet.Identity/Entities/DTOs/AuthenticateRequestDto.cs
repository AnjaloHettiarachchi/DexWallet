using System.ComponentModel.DataAnnotations;

namespace DexWallet.Identity.Entities.DTOs;

public class AuthenticateRequestDto
{
    [Required]
    public string Username { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}