using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Response;
using MediatR;
using TodoService.Services;

namespace TodoService.Features.Query
{
    
    public record GetTodoByDateQuery(Guid userId, DateTime @from, DateTime @to) : IRequest<ResultBase<IEnumerable<TB_TODO>>>;
    public class GetTodoByDateQueryHandler : IRequestHandler<GetTodoByDateQuery, ResultBase<IEnumerable<TB_TODO>>>
    {
        private readonly ITodoService _todoService;
        public GetTodoByDateQueryHandler(ITodoService todoService)
        {
            _todoService = todoService;
        }
        
        public async Task<ResultBase<IEnumerable<TB_TODO>>> Handle(GetTodoByDateQuery request, CancellationToken cancellationToken)
        {
            return await ResultBase<IEnumerable<TB_TODO>>.SuccessAsync(
                await _todoService.GetTodoByDateAsync(request.userId, request.from, request.to));
        }
    }
}