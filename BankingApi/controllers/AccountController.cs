using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BankingApi.Models;
using BankingSystem;

namespace BankingApi.Controllers;

[ApiController]
[Route("account")]
[Authorize]                          
public class AccountController : ControllerBase
{
    private readonly AccountRepository _repository = new AccountRepository(new Database());

    [HttpGet("balance")]
    public IActionResult Balance()
    {
        int id = int.Parse(User.FindFirst("id")!.Value);   
        Account? account = _repository.FindById(id);

        if (account == null)
            return NotFound("Account not found");

        return Ok(new { balance = account.Balance });
    }
    [HttpPost("deposit")]
    public IActionResult Deposit([FromBody] AmountRequest amount)
    {
        if (amount.Amount <= 0)
            return BadRequest("Invalid amount.");
        
        int id = int.Parse(User.FindFirst("id")!.Value);
        Account? account = _repository.FindById(id);
        if (account == null)
            return NotFound("Account not found");
        
        decimal newBalance =  account.Balance + amount.Amount;
         _repository.UpdateBalance(id, newBalance);


        return Ok( new {Balance = newBalance});
    }
    [HttpPost("withdraw")]
    public IActionResult Withdraw([FromBody] AmountRequest amount)
    {
        if (amount.Amount <= 0)
            return BadRequest("Invalid amount.");

        int id = int.Parse(User.FindFirst("id")!.Value);
        Account? account = _repository.FindById(id);
        if (account == null)
            return NotFound("Account not found");

        if ( account.Balance < amount.Amount){
            return BadRequest("Insufficient balance");
        }
        decimal newBalance = account.Balance - amount.Amount;
        _repository.UpdateBalance(id, newBalance);
        return Ok(new { Balance = newBalance });

    }

    [HttpPost("transfer")]
    public IActionResult Transfer([FromBody] TransferRequest request)
    {
        if (request.Amount <= 0)
            return BadRequest("Invalid amount.");

        if (request.CardNumber.Length != 16)
            return BadRequest("card number invalid");
        
        int id = int.Parse(User.FindFirst("id")!.Value);
        Account? account = _repository.FindById(id);
        if (account == null)
            return NotFound("Account not found");

        if (account.CardNumber == request.CardNumber)
            return BadRequest("You can't transfer to yourself.");

        if (account.Balance < request.Amount)
            return BadRequest("Insufficient balance");

        Account? accountCard = _repository.FindByCardNumber(request.CardNumber);
        if (accountCard == null)
            return BadRequest("card number invalid"); 
        
         if ( !_repository.Transfer(id, request.CardNumber, request.Amount))
            return StatusCode(500, "Transfer failed");
        return Ok(new {newBalance = account.Balance - request.Amount});

    }

    [HttpPost("close")]
    public IActionResult Clouse([FromBody] CloseRequest request)
    {
        
        int id = int.Parse(User.FindFirst("id")!.Value);
        Account? account = _repository.FindById(id);

        if (account == null)
            return NotFound("Account not found");

        if (account.Balance > 0)
             return BadRequest("You must withdraw or transfer your balance before closing.");
            
        if (!_repository.ValidPassword(account.Username, request.Password))
            return BadRequest("password invalid");

        _repository.DeleteAccount(id);
         
         return Ok( new { account = "cuenta eliminada"});
    }
}