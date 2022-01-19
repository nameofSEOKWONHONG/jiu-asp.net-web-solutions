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
    public record GetAllTodoQuery(Guid userId) : IRequest<Result<IEnumerable<TB_TODO>>>;
    
    public class GetAllTodoQueryHandler: IRequestHandler<GetAllTodoQuery, Result<IEnumerable<TB_TODO>>>
    {
        private readonly ITodoService _todoService;
        public GetAllTodoQueryHandler(ITodoService todoService)
        {
            _todoService = todoService;
        }
        public async Task<Result<IEnumerable<TB_TODO>>> Handle(GetAllTodoQuery request, CancellationToken cancellationToken)
        {
            return await Result<IEnumerable<TB_TODO>>.SuccessAsync(await _todoService.GetAllTodoAsync(request.userId));
        }
    }
}