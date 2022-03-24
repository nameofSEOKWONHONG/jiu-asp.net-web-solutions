using System.Linq.Expressions;
using System.Transactions;
using Application.Abstract;
using Application.Context;
using Domain.Dtos;
using eXtensionSharp;
using InjectionExtension;

namespace DomainServiceRepoSampleSvc;

public interface IPersonRepository : IRepositoryBase<PersonDto>
{
    
}

public interface IStudentRepository : IRepositoryBase<StudentDto>
{
}

[Transaction(TransactionScopeOption.Suppress)]
[AddService(ENUM_LIFE_TIME_TYPE.Scope, typeof(IPersonRepository))]
public class PersonRepository : RepositoryBase<ApplicationDbContext, PersonDto>, IPersonRepository 
{
    private readonly List<PersonDto> _items;
    public PersonRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public override PersonDto Fetch(PersonDto item)
    {
        var filters = new List<Func<PersonDto, bool>>();
        if(item.ID > 0) filters.Add(m => m.ID == item.ID);
        if(item.Name.xIsNotEmpty()) filters.Add(m => m.Name == item.Name);
        if(item.AGE > 0) filters.Add(m => m.AGE == item.AGE);

        IEnumerable<PersonDto> dummy = null;
        filters.ForEach(filter =>
        {
            dummy = _items.Where(filter);
        });
        return dummy.FirstOrDefault();
    }

    public override IEnumerable<PersonDto> Query(PersonDto request, int currentPage = 1, int pageSize = 50)
    {
        var filters = new List<Func<PersonDto, bool>>();
        if(request.Name.xIsNotEmpty()) filters.Add(m => m.Name.Contains(request.Name));
        if(request.AGE > 0) filters.Add(m => m.AGE == request.AGE);

        IEnumerable<PersonDto> dummy = null;
        filters.ForEach(filter =>
        {
            dummy = _items.Where(filter);
        });
        return dummy;
    }

    public override bool ComplexNonQuery(PersonDto entity)
    {
        return true;
    }
}

public class StudentRepository : RepositoryBase<ApplicationDbContext, StudentDto>, IStudentRepository
{
    private readonly IList<StudentDto> list = new List<StudentDto>();
    public StudentRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public override StudentDto Fetch(StudentDto request)
    {
        var filters = new List<Func<StudentDto, bool>>();
        if(request.PERSON.Name.xIsNotEmpty()) filters.Add(m => m.PERSON.Name == request.PERSON.Name); 
        if(request.ID > 0) filters.Add(m => m.ID == request.ID);
        if(request.STUDENT_NO.xIsNotEmpty()) filters.Add(m => m.STUDENT_NO == request.STUDENT_NO);
        IEnumerable<StudentDto> where = null;
        filters.xForEach(filter =>
        {
            where = list.Where(filter);
        });
        return where.FirstOrDefault();
    }

    public override IEnumerable<StudentDto> Query(StudentDto request, int currentPage = 1, int pageSize = 50)
    {
        return base.Query(request, currentPage, pageSize);
    }

    public override IEnumerable<StudentViewDto> ComplexQuery<StudentViewDto>(StudentDto entity)
    {
        var sql =
            $"SELECT * FROM STUDENT A INNER JOIN PERSON B ON A.PERSON_ID == B.ID WHERE A.ID = {entity.ID}";

        return null;
    }

    public override bool ComplexNonQuery(StudentDto entity)
    {
        return base.ComplexNonQuery(entity);
    }
}

public class StudentViewDto
{
    //person and student view properties
}