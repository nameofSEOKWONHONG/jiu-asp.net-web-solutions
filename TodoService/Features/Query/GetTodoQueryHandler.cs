using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces.Todo;
using Application.Response;
using Domain.Entities;
using MediatR;

namespace TodoService.Features.Query
{
    public record GetTodoQuery(int Id) : IRequest<Result<TB_TODO>>;

    public class GetTodoQueryHandler : IRequestHandler<GetTodoQuery, Result<TB_TODO>>
    {
        private readonly ITodoService _todoService;
        public GetTodoQueryHandler(ITodoService todoService)
        {
            _todoService = todoService;
        }
        
        public async Task<Result<TB_TODO>> Handle(GetTodoQuery request, CancellationToken cancellationToken)
        {
            return await Result<TB_TODO>.SuccessAsync(await _todoService.GetTodoAsync(request.Id));
        }
    }
}