using DexWallet.Common.Attributes;
using DexWallet.Common.Models.DTOs;
using DexWallet.Core.Contracts;
using DexWallet.Core.Entities.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DexWallet.Core.Controllers;

[Authorize]
[ApiController]
[Route("/wallets")]
public class WalletsController : ControllerBase
{
    private readonly IWalletService _walletService;

    public WalletsController(IWalletService walletService)
    {
        _walletService = walletService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllWallets()
    {
        var owner = GetUsernameFromContext();
        var wallets = await _walletService.GetAllWalletsByOwnerAsync(owner);
        return Ok(wallets);
    }

    [HttpGet("{walletAddress}")]
    public async Task<IActionResult> GetWalletByAddress(string walletAddress)
    {
        var owner = GetUsernameFromContext();
        var wallet = await _walletService.GetWalletByAddressAsync(owner, walletAddress);
        return Ok(wallet);
    }

    [HttpPost]
    public async Task<IActionResult> CreateWallet([FromBody] CreateWalletRequestDto request)
    {
        var owner = GetUsernameFromContext();
        var newWallet = await _walletService.CreateWalletAsync(owner, request.Name, request.RegularType, request.CryptoType, request.InitialRegularBalance, request.InitialCryptoBalance);
        return Ok(newWallet);
    }

    [HttpPost("store/{fundsType:required}")]
    public async Task<IActionResult> StoreFunds([FromBody] StoreFundsRequestDto request, string fundsType)
    {
        var owner = GetUsernameFromContext();
        var fundsUpdatedWallet = fundsType.Equals("regular")
            ? await _walletService.StoreRegularFundsAsync(owner, request.WalletAddress, request.Amount)
            : await _walletService.StoreCryptoFundsAsync(owner, request.WalletAddress, request.Amount);
        return Ok(fundsUpdatedWallet);
    }

    private string GetUsernameFromContext()
    {
        return ((IdentityValidateResult)HttpContext.Items["User"]!).Username;
    }
}