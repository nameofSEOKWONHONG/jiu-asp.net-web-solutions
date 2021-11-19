using WebApiApplication.Services.Abstract;

namespace WebApiApplication.Services
{
    public class GenerateViewService : IGenerateViewService
    {
        public string GetForm(string formId)
        {
            return "<button type=\"button\" class=\"mud-button-root mud-button mud-button-filled mud-button-filled-primary mud-button-filled-size-medium mud-ripple\"><span class=\"mud-button-label\">ClickMe</span></button>";
        }
    }
}