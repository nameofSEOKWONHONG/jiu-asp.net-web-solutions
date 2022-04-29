// See https://aka.ms/new-console-template for more information

using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.Data.SqlClient;
using SqlQuerySample.Sql;
using SqlQuerySampleApp.SampleEntity;
using ValidationResult = FluentValidation.Results.ValidationResult;


// var validators = new Dictionary<Type, IValidator>();
// validators.Add(typeof(TB_USER), new TB_USER.Validator());
// validators.Add(typeof(TB_PHONE), new TB_PHONE.Validator());
//
// var resolverBase = new ValidatorResolverBase();
//
//
// TB_USER user = new TB_USER() { ID = 0, NAME = "TEST" };
// var result = resolverBase.ExecuteCore(user);
// //var result = validators[user.GetType()].Validate(new ValidationContext<object>(user));
//
// //var validator = new TB_USER.Validator();
// //var result = validator.Validate(user);
//
// var phones = new List<TB_PHONE>()
// {
//     new TB_PHONE(){ID = 1, USER_ID = 1, NUMER = "00011112222", PHONE_TYPE = ENUM_PHONE_TYPE.MOBILE},
//     new TB_PHONE(){ID = 2, USER_ID = 1, NUMER = "021112222", PHONE_TYPE = ENUM_PHONE_TYPE.TEL},
// };
// phones.ForEach(item =>
// {
//     var validator = new TB_PHONE.Validator();
// });
//
// public class ValidatorResolverBase
// {
//     public Dictionary<Type, IValidator> _Validators = new();
//
//     public ValidatorResolverBase()
//     {
//         
//     }
//
//     private void Init()
//     {
//         //assembly load, base type and validation implement.
//         _Validators.Add(typeof(TB_USER), new TB_USER.Validator());
//         //
//     }
//
//     public ValidationResult ExecuteCore(object validationObject)
//     {
//         return _Validators[validationObject.GetType()].Validate(new ValidationContext<object>(validationObject));
//     }
// }

var sample = new QueryPocoSample();
sample.Run();