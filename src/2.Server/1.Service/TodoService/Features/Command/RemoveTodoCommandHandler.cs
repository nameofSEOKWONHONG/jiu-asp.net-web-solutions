using System.Threading;
using System.Threading.Tasks;
using Domain.Response;
using MediatR;
using TodoService.Services;

namespace TodoService.Features.Command
{
    public record RemoveTodoCommand(int id) : IRequest<Result<bool>>;
    public class RemoveTodoCommandHandler : IRequestHandler<RemoveTodoCommand, Result<bool>>
    {
        private readonly ITodoService _todoService;
        public RemoveTodoCommandHandler(ITodoService todoService)
        {
            _todoService = todoService;
        }
        public async Task<Result<bool>> Handle(RemoveTodoCommand request, CancellationToken cancellationToken)
        {
            return await Result<bool>.SuccessAsync(await _todoService.RemoveTodoAsync(request.id));
        }
    }
}