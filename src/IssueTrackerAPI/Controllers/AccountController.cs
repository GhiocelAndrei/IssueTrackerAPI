using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using IssueTracker.Application.Services;

namespace IssueTrackerAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly AccountService _accountService;

        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("callback")]
        public async Task Callback(string returnUrl = "/")
        {
            var authenticationProperties = new AuthenticationProperties() { RedirectUri = returnUrl };
            await HttpContext.ChallengeAsync("Auth0", authenticationProperties);
        }

        [HttpGet("getToken")]
        public async Task<IActionResult> GetToken()
        {
            var accessToken = await _accountService.GetToken();
            return Ok(accessToken);
        }
    }
}
