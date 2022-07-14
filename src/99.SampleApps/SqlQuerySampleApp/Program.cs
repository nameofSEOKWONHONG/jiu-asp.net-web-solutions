// See https://aka.ms/new-console-template for more information

using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using FluentValidation;
using Microsoft.Data.SqlClient;
using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;
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

// var sample = new QueryPocoSample();
// sample.Run();

var connection = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=JiuDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
var compiler = new SqlServerCompiler();
var db = new QueryFactory(connection, compiler);
db.Logger += result => Console.WriteLine(result.ToString()); 

var id = "34E98B0E-620B-4280-81F3-08D9D2BAFA73";
var query = db.Query("TB_USER")
    .Where("ID", "=", id)
    .Limit(1);

//compile sql
SqlResult result = compiler.Compile(query);
//write sql parameterize
Console.WriteLine(result.Sql);
//result
IEnumerable<dynamic> dataResult = query.Get();
var selected = dataResult.First();
Console.WriteLine(selected.ID);
