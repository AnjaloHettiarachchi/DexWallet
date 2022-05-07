using DexWallet.Common.Attributes;
using DexWallet.Exchange.Contracts;
using DexWallet.Exchange.Entities.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DexWallet.Exchange.Controllers;

[Authorize]
[ApiController]
[Route("/exchange")]
public class ExchangesController : ControllerBase
{
    private readonly IExchangeService _exchangeService;

    public ExchangesController(IExchangeService exchangeService)
    {
        _exchangeService = exchangeService;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ExchangeRequestDto request)
    {
        var authToken = GetAuthTokenFromRequest();
        var wallet = await _exchangeService.DoExchange(authToken, request.WalletAddress, request.FromType, request.Amount);
        return Ok(wallet);
    }

    private string GetAuthTokenFromRequest()
    {
        return Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last() ?? string.Empty;
    }
}