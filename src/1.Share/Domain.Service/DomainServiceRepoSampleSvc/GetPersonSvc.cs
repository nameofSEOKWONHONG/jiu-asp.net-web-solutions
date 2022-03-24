using System.Transactions;
using Application.Abstract;
using Domain.Dtos;
using eXtensionSharp;
using InjectionExtension;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Ocsp;
using WebApiApplication.Services.Abstract;

namespace DomainServiceRepoSampleSvc;

public interface IGetPersonSvc : IServiceBase<int, PersonDto>
{
    
}

[Transaction(TransactionScopeOption.Suppress)]
[AddService(ENUM_LIFE_TIME_TYPE.Scope, typeof(IGetPersonSvc))]
public class GetPersonSvc : ServiceBase<int, PersonDto>, IGetPersonSvc
{
    private readonly IPersonRepository _personRepository;
    public GetPersonSvc(ILogger logger, ISessionContext sessionContext, 
        IPersonRepository personRepository) : base(logger, sessionContext)
    {
        _personRepository = personRepository;
    }

    protected override PersonDto OnExecute(ISessionContext sessionContext, int Request)
    {
        _personRepository.Create(new PersonDto());
        _personRepository.ComplexNonQuery(new PersonDto());
        return _personRepository.Fetch(new PersonDto() { ID = Request });
    }
}

public interface IGetPersonByStudentSvc : IServiceBase<int, PersonDto>
{
}

[Transaction(TransactionScopeOption.Suppress)]
[AddService(ENUM_LIFE_TIME_TYPE.Scope, typeof(IGetPersonByStudentSvc))]
public class GetPersonByStudentSvc : ServiceBase<int, PersonDto>, IGetPersonByStudentSvc
{
    private readonly IPersonRepository _personRepository;
    private readonly IStudentRepository _studentRepository;
    public GetPersonByStudentSvc(ILogger logger, ISessionContext sessionContext, 
        IPersonRepository personRepository,
        IStudentRepository studentRepository) : base(logger, sessionContext)
    {
        _personRepository = personRepository;
        _studentRepository = studentRepository;
    }

    protected override PersonDto OnExecute(ISessionContext sessionContext, int request)
    {
        var exists = _studentRepository.Fetch(new StudentDto() { ID = request });
        if (exists.xIsNotEmpty())
        {
            return exists.PERSON;
        }

        return null;
    }
}

public class GetStudentSummarySvc : ServiceBase<int, IEnumerable<StudentDto>>
{
    public GetStudentSummarySvc(ILogger logger, ISessionContext sessionContext) : base(logger, sessionContext)
    {
    }

    protected override IEnumerable<StudentDto> OnExecute(ISessionContext sessionContext, int request)
    {
        throw new NotImplementedException();
    }
} 

