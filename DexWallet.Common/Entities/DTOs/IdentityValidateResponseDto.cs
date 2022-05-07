namespace DexWallet.Common.Models.DTOs;

public class IdentityValidateResult
{
    public string Username { get; init; } = null!;

    public string FirstName { get; init; } = null!;

    public string LastName { get; init; } = null!;
}

public class IdentityValidateResponseDto
{
    public IdentityValidateResponseDto(IdentityValidateResult? result, bool isSuccess, string message)
    {
        IsSuccess = isSuccess;
        Message = message;
        Result = result;
    }

    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public IdentityValidateResult? Result { get; set; }
}