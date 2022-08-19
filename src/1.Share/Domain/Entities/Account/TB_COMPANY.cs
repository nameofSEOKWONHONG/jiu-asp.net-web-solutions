using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("TB_COMPANY")]
public class TB_COMPANY : GuidEntityBase
{
    [Required]
    public string COM_NM { get; set; }
    [Required]
    public string OWNER_NM { get; set; }
    [Required]
    public string ADDR { get; set; }
    [Required]
    public string ADDR_DETAIL { get; set; }
    
    public TB_CONNECT CONNECT { get; set; }
}