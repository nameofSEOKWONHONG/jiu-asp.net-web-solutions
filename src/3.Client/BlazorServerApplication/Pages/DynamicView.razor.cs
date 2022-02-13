using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace BlazorServerApplication.Pages
{
    public class DynamicRenderComponent
    {
        public Type WigetType { get; set; }
        public Dictionary<string, object> Properties { get; set; }
        public Action Event { get; set; }
    }
    
    public partial class DynamicView : IAsyncDisposable
    {
        private MarkupString _button;

        private readonly List<DynamicRenderComponent> _components = new List<DynamicRenderComponent>();
        protected override async Task OnInitializedAsync()
        {
            _components.Add(new DynamicRenderComponent()
            {
                WigetType = typeof(MudButton),
                Properties = new Dictionary<string, object>()
                {
                    ["Text"] = "ClickMe",
                    ["Variant"] = Variant.Filled,
                    ["Color"] = Color.Primary
                },
                Event = (async () =>
                {
                    var js = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "./Pages/DynamicView.razor.js");
                    await js.InvokeVoidAsync("error");
                })
            });
            _components.Add(new DynamicRenderComponent()
            {
                WigetType = typeof(MudButton),
                Properties = new Dictionary<string, object>()
                {
                    ["Text"] = "ClickMe2",
                    ["Variant"] = Variant.Filled,
                    ["Color"] = Color.Primary
                },
                Event = (async () =>
                {
                    await Task.CompletedTask;
                })
            });            
            
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:5001");
                var response = await client.GetAsync("api/v1/GenerateView");
                if (response.IsSuccessStatusCode)
                {
                    var str = await response.Content.ReadAsStringAsync();
                    _button = new MarkupString(str);
                }
            }
        }
        
        private async Task OnClick()
        {
            var js = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "./Pages/DynamicView.razor.js");
            await js.InvokeVoidAsync("error");
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}