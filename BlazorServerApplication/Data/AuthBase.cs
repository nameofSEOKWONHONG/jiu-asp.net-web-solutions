using System;
using System.Text.Json.Serialization;

namespace BlazorServerApplication.Data
{
    public class OAuthInfo
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string AutheticatiotnType { get; set; }
        public bool IsAuthenticated { get; set; }
        public Int64 AuthId { get; set; }
        public DateTime AuthAt { get; set; }
    }
}