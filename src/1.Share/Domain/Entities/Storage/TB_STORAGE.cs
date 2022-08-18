using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities.Storage;

[Table("TB_STORAGE")]
[Index(nameof(FILE_NAME), nameof(CLOUD_TYPE), IsUnique = false)]
public class TB_STORAGE : GuidEntityBase
{
    [Required]
    public string FILE_NAME { get; set; }
    [Required]
    public string FILE_URL { get; set; }
    [Required]
    public long FILE_SIZE { get; set; }
    [Required]
    public short CLOUD_TYPE { get; set; }
}