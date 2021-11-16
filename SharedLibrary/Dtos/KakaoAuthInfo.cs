using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharedLibrary.Dtos
{
    public class KakaoAuthInfo
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("connected_at")]
        public string ConnectedAt { get; set; }

        [JsonPropertyName("properties")]
        public Properties Properties { get; set; }

        [JsonPropertyName("kakao_account")]
        public KakaoAccount KakaoAccount { get; set; }

    }

    public class KakaoAccount
    {

        [JsonPropertyName("profile_needs_agreement")]
        public bool ProfileNeedsAgreement { get; set; }

        [JsonPropertyName("profile")]
        public Profile Profile { get; set; }

        [JsonPropertyName("has_email")]
        public bool HasEmail { get; set; }

        [JsonPropertyName("email_needs_agreement")]
        public bool EmailNeedsAgreement { get; set; }

        [JsonPropertyName("is_email_valid")]
        public bool IsEmailValid { get; set; }

        [JsonPropertyName("is_email_verified")]
        public bool IsEmailVerified { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("has_age_range")]
        public bool HasAgeRange { get; set; }

        [JsonPropertyName("age_range_needs_agreement")]
        public bool AgeRangeNeedsAgreement { get; set; }

        [JsonPropertyName("age_range")]
        public string AgeRange { get; set; }

        [JsonPropertyName("has_birthday")]
        public bool HasBirthday { get; set; }

        [JsonPropertyName("birthday_needs_agreement")]
        public bool BirthdayNeedsAgreement { get; set; }

        [JsonPropertyName("birthday")]
        public string Birthday { get; set; }

        [JsonPropertyName("birthday_type")]
        public string BirthdayType { get; set; }

        [JsonPropertyName("has_gender")]
        public bool HasGender { get; set; }

        [JsonPropertyName("gender_needs_agreement")]
        public bool GenderNeedsAgreement { get; set; }

        [JsonPropertyName("gender")]
        public string Gender { get; set; }
    }
    
    public class Profile
    {

        [JsonPropertyName("nickname")]
        public string Nickname { get; set; }

        [JsonPropertyName("thumbnail_image_url")]
        public string ThumbnailImageUrl { get; set; }

        [JsonPropertyName("profile_image_url")]
        public string ProfileImageUrl { get; set; }

        [JsonPropertyName("is_default_image")]
        public bool IsDefaultImage { get; set; }
    }
    
    public class Properties
    {

        [JsonPropertyName("nickname")]
        public string Nickname { get; set; }

        [JsonPropertyName("profile_image")]
        public string ProfileImage { get; set; }

        [JsonPropertyName("thumbnail_image")]
        public string ThumbnailImage { get; set; }
    }
    
}