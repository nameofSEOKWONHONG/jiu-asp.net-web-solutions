using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces.Todo;
using Application.Response;
using Domain.Entities;
using MediatR;

namespace TodoApplication.Features.Query
{
    public record GetAllTodoQuery(Guid userId) : IRequest<Result<IEnumerable<Todo>>>;
    
    public class GetAllTodoQueryHandler: IRequestHandler<GetAllTodoQuery, Result<IEnumerable<Todo>>>
    {
        private readonly ITodoService _todoService;
        public GetAllTodoQueryHandler(ITodoService todoService)
        {
            _todoService = todoService;
        }
        public async Task<Result<IEnumerable<Todo>>> Handle(GetAllTodoQuery request, CancellationToken cancellationToken)
        {
            return await Result<IEnumerable<Todo>>.SuccessAsync(await _todoService.GetAllTodoAsync(request.userId));
        }
    }
}