using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.KeyValueStore;

[Table("TB_GRP_KV_STORE")]
public class TB_GRP_KV_STORE : EntityBase
{
    [Key, Column(Order = 0)]
    public string COM_CD { get; set; }
    [Key, Column(Order = 1)]
    public string GRP_CD { get; set; }
    [Key, Column(Order = 2)]
    public string GRP_KEY { get; set; }
    [Required]
    public string GRP_VAL { get; set; }
}