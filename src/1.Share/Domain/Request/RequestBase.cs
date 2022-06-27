namespace Domain.Request;

public interface IRequestBase<T>
{
    T Data { get; set; }
}

public class RequestBase<T> : IRequestBase<T>
{
    public T Data { get; set; }
}

public class PagingRequestBase<T> : RequestBase<T>
{
    public int PageSize { get; set; }
    public int CurrentPage { get; set; }
    public string[] OrderBy { get; set; }
}