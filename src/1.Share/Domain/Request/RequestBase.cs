namespace Domain.Request;

public interface IRequestBase<T>
{
    T Data { get; set; }
}

public class RequestBase<T> : IRequestBase<T>
{
    public T Data { get; set; }
}