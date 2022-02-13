using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlazorServerApplication.Pages
{
    public class Login : PageModel
    {
        public async Task OnGet(string redirectUri)
        {
            await HttpContext.ChallengeAsync("KakaoTalk", new AuthenticationProperties
            {
                RedirectUri = redirectUri
            });
        }
    }
}