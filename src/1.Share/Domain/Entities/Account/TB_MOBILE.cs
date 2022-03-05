using System.ComponentModel.DataAnnotations;
using FluentValidation;
using PhoneNumbers;

namespace Domain.Entities;

public class TB_CONNECT : AutoIncEntityBase
{
    public TB_EMAIL EMAIL { get; set; }
    public TB_MOBILE MOBILE { get; set; }
    
    public class Validator : AbstractValidator<TB_CONNECT>
    {
        public Validator()
        {
            RuleFor(m => m.MOBILE).SetValidator(new MobileValidator());
        }
    }
}

public class TB_MOBILE : AutoIncEntityBase
{
    [Required, MaxLength(2)]
    public string NATION { get; set; }
    [Required, MaxLength(15)]
    public string MOBILE { get; set; }
    
    public class Validator : AbstractValidator<TB_MOBILE>
    {
        public Validator()
        {
            RuleFor(m => m.NATION).NotEmpty();
            RuleFor(m => m.MOBILE).NotEmpty();
        }
    }
}

public class TB_EMAIL : AutoIncEntityBase
{
    [Required, MaxLength(20)]
    public string DOMAIN { get; set; }
    [Required, MaxLength(50)]
    public string FRONT_TEXT { get; set; }
    public string EMAIL
    {
        get => $"{FRONT_TEXT}@{DOMAIN}";
    }

    public class Validator : AbstractValidator<TB_EMAIL>
    {
        public Validator()
        {
            RuleFor(m => m.DOMAIN).NotEmpty();
            RuleFor(m => m.FRONT_TEXT).NotEmpty();
        }
    }
}

public class MobileValidator : AbstractValidator<TB_MOBILE>
{
    public MobileValidator()
    {
        RuleFor(m => m.MOBILE).NotEmpty();
        RuleFor(m => m.NATION).NotEmpty().When(m =>
        {
            var util = PhoneNumberUtil.GetInstance();
            var number = util.Parse(m.MOBILE, m.NATION);
            return PhoneNumberUtil.IsViablePhoneNumber(number.RawInput);
        });
    }
}

public class EmailValidator : AbstractValidator<TB_EMAIL>
{
    public EmailValidator()
    {
        
    }
}