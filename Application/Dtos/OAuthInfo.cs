using System;
using System.Text.Json.Serialization;

namespace Application.Dtos
{
    public record OAuthInfo<TAuthEntity>
    {
        public string AccessToken { get; init; }
        public string RefreshToken { get; init; }
        public string AutheticatiotnType { get; init; }
        public bool IsAuthenticated { get; init; }
        public TAuthEntity AuthEntity { get; set; }

        public OAuthInfo(string accessToken, string refreshToken, string autheticatiotnType, bool isAuthenticated)
        {
            this.AccessToken = accessToken;
            this.RefreshToken = refreshToken;
            this.AutheticatiotnType = autheticatiotnType;
            this.IsAuthenticated = isAuthenticated;
        }
    }
}