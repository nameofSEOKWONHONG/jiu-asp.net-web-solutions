using System.ComponentModel.DataAnnotations;
using System.Data;
using FluentValidation;

namespace SqlQuerySampleApp.SampleEntity;

public class TB_USER
{
    public int ID { get; set; }
    public string NAME { get; set; }
    public class Validator : AbstractValidator<TB_USER>
    {
        public Validator()
        {
            RuleFor(m => m.ID).GreaterThan(0);
            RuleFor(m => m.NAME).NotNull();
        }
    }
}

public class TB_PHONE
{
    [Key]
    public int ID { get; set; }
    [Key]
    public ENUM_PHONE_TYPE PHONE_TYPE { get; set; }
    [Key]
    public int USER_ID { get; set; }
    [Required]
    public string NUMER { get; set; }
    
    public class Validator : AbstractValidator<TB_PHONE>
    {
        public Validator()
        {
            RuleFor(m => m.ID).GreaterThan(0);
            RuleFor(m => m.PHONE_TYPE).Equals(new[] { ENUM_PHONE_TYPE.TEL, ENUM_PHONE_TYPE.MOBILE });
            RuleFor(m => m.USER_ID).NotNull();
            RuleFor(m => m.NUMER).NotNull();
        }
    }
}

public class TB_GRADE
{
    public int ID { get; set; }
    public int USER_ID { get; set; }
    public string GRADE { get; set; }
}

public enum ENUM_PHONE_TYPE
{
    MOBILE,
    TEL
}