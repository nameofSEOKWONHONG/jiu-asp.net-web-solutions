using Application.Interfaces.HelloWorld;
using Domain.Response;
using MediatR;

namespace HelloWorldService.Features.Queries
{
    public record GetHelloWorldDataQuery() : IRequest<Result<string>>;
    public class GetHelloWorldDataHandler : IRequestHandler<GetHelloWorldDataQuery, Result<string>>
    {
        private readonly IHelloWorldService _helloWorldService;
        public GetHelloWorldDataHandler(IHelloWorldService helloWorldService)
        {
            _helloWorldService = helloWorldService;
        }
        public Task<Result<string>> Handle(GetHelloWorldDataQuery request, CancellationToken cancellationToken)
        {
            return Result<string>.SuccessAsync(data:_helloWorldService.HelloWorld());
        }
    }
}