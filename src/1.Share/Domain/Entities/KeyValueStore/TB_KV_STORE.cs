using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.KeyValueStore;

[Table("TB_KV_STORE")]
public class TB_KV_STORE : EntityBase
{
    [Key]
    public int ID { get; set; }
    [Key]
    public string KEY { get; set; }
    public string VAL { get; set; }
    
    [NotMapped]
    public string KV { get; set; }
}