using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.KeyValueStore;

[Table("TB_KV_STORE")]
public class TB_KV_STORE : EntityBase
{
    [Key, Column(Order = 0)]
    public int ID { get; set; }
    [Key, Column(Order = 1)]
    public string KEY { get; set; }
    [Required]
    public string VAL { get; set; }
    
    [NotMapped]
    public string KvComposite { get; set; }
}