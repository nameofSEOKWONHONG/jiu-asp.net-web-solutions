using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace BlazorServerApplication.Pages
{
    public partial class Index : IAsyncDisposable
    {
        protected override Task OnInitializedAsync()
        {
            return base.OnInitializedAsync();
        }
        
        private async Task ClickEvent()
        {
            var js = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "./Pages/Index.razor.js");
            await js.InvokeVoidAsync("error");
        }        

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}