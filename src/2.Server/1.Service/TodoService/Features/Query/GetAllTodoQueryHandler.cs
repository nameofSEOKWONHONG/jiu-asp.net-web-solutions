﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Response;
using MediatR;
using TodoService.Services;

namespace TodoService.Features.Query
{
    public record GetAllTodoQuery(Guid userId) : IRequest<ResultBase<IEnumerable<TB_TODO>>>;
    
    public class GetAllTodoQueryHandler: IRequestHandler<GetAllTodoQuery, ResultBase<IEnumerable<TB_TODO>>>
    {
        private readonly ITodoService _todoService;
        public GetAllTodoQueryHandler(ITodoService todoService)
        {
            _todoService = todoService;
        }
        public async Task<ResultBase<IEnumerable<TB_TODO>>> Handle(GetAllTodoQuery request, CancellationToken cancellationToken)
        {
            return await ResultBase<IEnumerable<TB_TODO>>.SuccessAsync(await _todoService.GetAllTodoAsync(request.userId));
        }
    }
}