using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces.Todo;
using Application.Response;
using Domain.Entities;
using MediatR;

namespace TodoApplication.Features.Query
{
    public record GetTodoQuery(int Id) : IRequest<Result<Todo>>;

    public class GetTodoQueryHandler : IRequestHandler<GetTodoQuery, Result<Todo>>
    {
        private readonly ITodoService _todoService;
        public GetTodoQueryHandler(ITodoService todoService)
        {
            _todoService = todoService;
        }
        
        public async Task<Result<Todo>> Handle(GetTodoQuery request, CancellationToken cancellationToken)
        {
            return await Result<Todo>.SuccessAsync(await _todoService.GetTodoAsync(request.Id));
        }
    }
}