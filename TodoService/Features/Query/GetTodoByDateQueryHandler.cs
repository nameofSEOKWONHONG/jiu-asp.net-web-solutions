using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces.Todo;
using Application.Response;
using Domain.Entities;
using MediatR;

namespace TodoApplication.Features.Query
{
    
    public record GetTodoByDateQuery(Guid userId, DateTime @from, DateTime @to) : IRequest<Result<IEnumerable<TB_TODO>>>;
    public class GetTodoByDateQueryHandler : IRequestHandler<GetTodoByDateQuery, Result<IEnumerable<TB_TODO>>>
    {
        private readonly ITodoService _todoService;
        public GetTodoByDateQueryHandler(ITodoService todoService)
        {
            _todoService = todoService;
        }
        
        public async Task<Result<IEnumerable<TB_TODO>>> Handle(GetTodoByDateQuery request, CancellationToken cancellationToken)
        {
            return await Result<IEnumerable<TB_TODO>>.SuccessAsync(
                await _todoService.GetTodoByDateAsync(request.userId, request.from, request.to));
        }
    }
}