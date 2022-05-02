namespace DexWallet.Common;

public class AppResponse
{
    public AppResponse(object? result, bool isSuccess = true, string message = "success")
    {
        Result = result;
        IsSuccess = isSuccess;
        Message = message;
    }

    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public object? Result { get; set; }
}