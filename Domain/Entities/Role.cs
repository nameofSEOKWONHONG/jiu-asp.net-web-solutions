using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using eXtensionSharp;

namespace Domain.Entities
{
    [Table("TB_ROLE")]
    public class Role : EntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required, MaxLength(10)]
        public ENUM_ROLE_TYPE RoleType { get; set; }
        
        public RoleClaim RoleClaim { get; set; }
    }

    [Table("TB_ROLE_CLAIM")]
    public class RoleClaim : EntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required, MaxLength(100)]
        public List<ENUM_ROLE_CLAIM_TYPE> RoleClaimTypes { get; set; }
    }

    public class ENUM_ROLE_TYPE : XEnumBase<ENUM_ROLE_TYPE>
    {
        public static readonly ENUM_ROLE_TYPE SUPER = Define("SUPER");
        public static readonly ENUM_ROLE_TYPE ADMIN = Define("ADMIN");
        public static readonly ENUM_ROLE_TYPE USER = Define("USER");
        public static readonly ENUM_ROLE_TYPE GUEST = Define("GUEST");
    }

    public class ENUM_ROLE_CLAIM_TYPE : XEnumBase<ENUM_ROLE_CLAIM_TYPE>
    {
        public static readonly ENUM_ROLE_CLAIM_TYPE V = Define("VIEW"); //readonly
        public static readonly ENUM_ROLE_CLAIM_TYPE C = Define("CREATE");
        public static readonly ENUM_ROLE_CLAIM_TYPE U = Define("UPDATE");
        public static readonly ENUM_ROLE_CLAIM_TYPE D = Define("DELETE");
    }
}