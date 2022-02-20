﻿using System.Text;
using eXtensionSharp;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using SpectreConsoleApplication.Menus.Abstract;

namespace SpectreConsoleApplication.Menus;

public class LoginAction : ActionBase
{
    public LoginAction(ILogger<LoginAction> logger, 
        IHttpClientFactory clientFactory) : base(logger, clientFactory)
    {
    }

    public async Task<string> LoginAsync(string email, string password)
    { 
        var dic = new Dictionary<string, string>()
        {
            { nameof(email), email },
            { nameof(password), password }
        };
        
        var request = new HttpRequestMessage(HttpMethod.Post,
            "https://localhost:5001/api/v1/Account/SignIn");
        request.Content = new StringContent(dic.xToJson(), Encoding.UTF8, "application/json");
        
        using var client = _clientFactory.CreateClient();
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var token = await response.Content.ReadAsStringAsync();
        return token;
    }
}