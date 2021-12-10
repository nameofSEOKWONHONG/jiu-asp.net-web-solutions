using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Entities
{
    [Table("TB_ROLE_PERMISSION")]
    public class RolePermission : EntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required, MaxLength(100)]
        public List<ENUM_ROLE_PERMISSION_TYPE> RolePermissionTypes { get; set; }
    }
}