﻿using System;
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
    public record GetTodoBySelectedDateQuery(Guid userId, DateTime selectedDate) : IRequest<Result<IEnumerable<Todo>>>;
    public class GetTodoBySelectedDataQueryHandler : IRequestHandler<GetTodoBySelectedDateQuery, Result<IEnumerable<Todo>>>
    {
        private readonly ITodoService _todoService;
        public GetTodoBySelectedDataQueryHandler(ITodoService todoService)
        {
            _todoService = todoService;
        }
        public async Task<Result<IEnumerable<Todo>>> Handle(GetTodoBySelectedDateQuery request, CancellationToken cancellationToken)
        {
            return await Result<IEnumerable<Todo>>.SuccessAsync(
                await _todoService.GetTodoByDateAsync(request.userId, request.selectedDate));
        }
    }
}