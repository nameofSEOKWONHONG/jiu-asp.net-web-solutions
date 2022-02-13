using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.System.Config;

[Table(nameof(TB_MIGRAION))]
public class TB_MIGRAION : AutoIncEntityBase
{
    public bool MIGRATION_YN { get; set; }
    public bool COMPLETE_YN { get; set; }
}