using Application.Interfaces.HelloWorld;

namespace HelloWorldApplication.Services
{
    public class HelloWorldService : IHelloWorldService
    {
        public string HelloWorld()
        {
            return "HelloWorld";
        }
    }
}