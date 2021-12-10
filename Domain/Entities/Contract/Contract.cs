using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Contract
{
    /// <summary>
    /// 계약 기록 테이블, 계약 발행시 기록하고 해당 계약이 무산되거나 다시 시도될 경우 원본 계약을 보존하기 위함.
    /// </summary>
    [Table("TB_CONTRACT_HISTORY")]
    public class ContractHistory : EntityBase
    {
        /// <summary>
        /// 원 계약 (원 계약과 상위 계약이 같을 경우 원본 계약이다)
        /// </summary>
        [Key, ForeignKey(nameof(Contract))]
        public Contract SrcContract { get; set; }
        
        /// <summary>
        /// 계약차수, 만약 한번에 계약한다면 1, 이후라면 계속 증가
        /// </summary>
        public int ContractCount { get; set; }
        
        /// <summary>
        /// 상위 계약 (원 계약과 상위 계약이 같을 경우 원본 계약이다) 
        /// </summary>
        public Contract ParentContract { get; set; }
        
        /// <summary>
        /// 하위 계약
        /// </summary>
        public Contract ChildContract { get; set; }
    }
    
    
    /// <summary>
    /// 계약 테이블, 계약 발행의 주 테이블로써 계약의 발행 및 체결을 기록한다.
    /// 단, 계약이 무산된 경우 기존 계약을 수정하지 않고 신규 계약으로 진행하도록 한다.
    /// 각 계약은 계약 기록 테이블에 남겨져야 한다.
    /// </summary>
    [Table("TB_CONTRACT")]
    public class Contract : EntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ContractId { get; set; }
        /// <summary>
        /// 발행자
        /// </summary>
        [ForeignKey("User")]
        public Guid ContractSubjectId { get; set; }
        /// <summary>
        /// 수신자
        /// </summary>
        [ForeignKey("User")]
        public Guid ContractPartnerId { get; set; }
        /// <summary>
        /// 발행일
        /// </summary>
        public DateTime PublishDate { get; set; }
        /// <summary>
        /// 수신자의 사인일
        /// </summary>
        public DateTime SignDate { get; set; }
        /// <summary>
        /// 계약서
        /// </summary>
        public ContractDoc ContractDoc { get; set; }
    }

    /// <summary>
    /// 계약 문서 테이블
    /// </summary>
    [Table("TB_CONTRACT_DOC")]
    public class ContractDoc : EntityBase
    {
        /// <summary>
        /// 문서번호
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid DocId { get; set; }
        /// <summary>
        /// 계약서 내용
        /// </summary>
        [Required]
        public string Contents { get; set; }
        /// <summary>
        /// 계약서 필드 아이템
        /// </summary>
        [Required]
        public List<ContractDocItem> ContractDocItems { get; set; }
    }

    /// <summary>
    /// 계약 문서 아이템 테이블
    /// </summary>
    [Table("TB_CONTRACT_DOC_ITEM")]
    public class ContractDocItem : EntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        /// <summary>
        /// 필드명
        /// </summary>
        [Required]
        public string Field { get; set; }
        /// <summary>
        /// 필드값
        /// </summary>
        [StringLength(200)]
        public string FieldValue { get; set; }
    }
}