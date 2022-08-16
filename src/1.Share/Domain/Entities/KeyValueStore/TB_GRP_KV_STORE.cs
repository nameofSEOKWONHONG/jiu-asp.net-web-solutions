using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.KeyValueStore;

[Table("TB_GRP_KV_STORE")]
public class TB_GRP_KV_STORE : EntityBase
{
    [Key]
    public string COM_CD { get; set; }
    [Key]
    public string GRP_CD { get; set; }
    public string GRP_KEY { get; set; }
    public string GRP_VAL { get; set; }
}