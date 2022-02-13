using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Entities
{
    [Table(nameof(TB_ROLE_PERMISSION))]
    public class TB_ROLE_PERMISSION : AutoIncEntityBase
    {
        [Required, MaxLength(100)]
        public List<ENUM_ROLE_PERMISSION_TYPE> ROLE_PERMISSION_TYPES { get; set; }
    }
}