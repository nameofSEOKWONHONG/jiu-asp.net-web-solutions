using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities.Document;

[Index(nameof(DOC_ID), nameof(VER), IsUnique = false)]
public class TB_DOC_MGR : GuidEntityBase
{
    /// <summary>
    /// 문서 ID
    /// </summary>
    [Required]
    [Key, Column(Order = 1)]
    public string DOC_ID { get; set; }
    
    /// <summary>
    /// 문서 버전
    /// </summary>
    [Required]
    public int VER { get; set; }
}