using System.Text.Json.Serialization;
using eXtensionSharp;

namespace Domain.Enums
{
    [JsonConverter(typeof(XEnumBaseJsonConverter<ENUM_ROLE_TYPE>))]
    public class ENUM_ROLE_TYPE : XEnumBase<ENUM_ROLE_TYPE>
    {
        /// <summary>
        /// 슈퍼 관리자 (슈퍼 관리자는 유일하게 존재해야 함)
        /// </summary>
        public static readonly ENUM_ROLE_TYPE SUPER = Define("SUPER");
        /// <summary>
        /// 일반 관리자 (다수의 일반 관리자가 존재할 수 있음)
        /// </summary>
        public static readonly ENUM_ROLE_TYPE ADMIN = Define("ADMIN");
        /// <summary>
        /// 사용자 (승인된 사용자 또는 정회원)
        /// </summary>
        public static readonly ENUM_ROLE_TYPE USER = Define("USER");
        /// <summary>
        /// 손님 (회원가입을 했으나 승인되지 않은 상태 또는 정회원이 아닌 상태)
        /// </summary>
        public static readonly ENUM_ROLE_TYPE GUEST = Define("GUEST");
    }
}