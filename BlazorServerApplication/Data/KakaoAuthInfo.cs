using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorServerApplication.Data
{
    public class KakaoAuthInfo
    {
        [JsonPropertyName("id")] 
        public Int64 Id { get; set; }

        [JsonPropertyName("connected_at")]
        public DateTime ConnectedAt { get; set; }
    }
}