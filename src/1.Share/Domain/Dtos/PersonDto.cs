using System;
using FluentValidation;

namespace Domain.Dtos;

public class PersonDto
{
    public int ID { get; set; }
    public string Name { get; set; }
    public int AGE { get; set; }
    
    public class Validator : AbstractValidator<PersonDto>
    {
        public Validator()
        {
            RuleFor(m => m.Name).NotNull();
            RuleFor(m => m.AGE).GreaterThan(0);
            RuleFor(m => m.Name).MinimumLength(2);
            RuleFor(m => m.Name).NotEqual("test");
        }
    }
}

public class StudentDto
{
    public int ID { get; set; }
    public string CLASS_NAME { get; set; }
    public string STUDENT_NO { get; set; }
    public DateTime ENTER_DATE { get; set; }
    public DateTime GRADUATE_DATE { get; set; }
    public PersonDto PERSON { get; set; }
}