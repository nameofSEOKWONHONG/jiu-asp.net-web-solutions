﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces.Todo;
using Application.Response;
using Domain.Entities;
using MediatR;

namespace TodoService.Features.Query
{
    public record GetTodoBySelectedDateQuery(Guid userId, DateTime selectedDate) : IRequest<Result<IEnumerable<TB_TODO>>>;
    public class GetTodoBySelectedDataQueryHandler : IRequestHandler<GetTodoBySelectedDateQuery, Result<IEnumerable<TB_TODO>>>
    {
        private readonly ITodoService _todoService;
        public GetTodoBySelectedDataQueryHandler(ITodoService todoService)
        {
            _todoService = todoService;
        }
        public async Task<Result<IEnumerable<TB_TODO>>> Handle(GetTodoBySelectedDateQuery request, CancellationToken cancellationToken)
        {
            return await Result<IEnumerable<TB_TODO>>.SuccessAsync(
                await _todoService.GetTodoByDateAsync(request.userId, request.selectedDate));
        }
    }
}