using HotChocolate.Subscriptions;
using HotChocolateSample.Entity;

namespace HotChocolateSample.GraphQL;

public interface IQuery
{
    IEnumerable<Book> GetBooks();
    Book? GetBookByTitle(string title);
    Book? GetBookByName(string name);
}

public class Query : IQuery
{
    private readonly BookService _service;
    public Query([Service]BookService bookService)
    {
        _service = bookService;
    }
/*
{
  books{
    title
    author {
      name
    }
  }
}
 */
    public IEnumerable<Book> GetBooks() => _service.GetBooks();
    /*
{
  bookByTitle(title: "C# in depth.") {
      title
      author {
        name
      }
  }
  bookByName(name: "Jon Skeet") {
      title
      author {
        name
      }    
  }
}
     */    
    public Book? GetBookByTitle(string title) => _service.GetBookByTitle(title);
    public Book? GetBookByName(string name) => _service.GetBookByName(name);
    //public Book? AddBook(Book item) => _service.AddBook(item);
}

public class Mutation
{
    private readonly BookService _service;
    public Mutation([Service(ServiceKind.Default)] BookService service)
    {
        _service = service;
    }
    
    /*
mutation {
  addBook(book: {
    title:"test2"
    author: {
      name:"test2"
    }
  }) {
    title
    author {
      name
    }
  }  
}
     */
    public async Task<Book?> AddBook(Book book, [Service] ITopicEventSender sender)
    {
        await sender.SendAsync("OnAddBook", book);
        return await Task.Factory.StartNew(() => _service.AddBook(book));
    }

    /*
mutation {
  removeBook(title:"test2")
}
     */
    public async Task<bool> RemoveBook(string title, [Service] ITopicEventSender sender)
    {
        await sender.SendAsync("OnRemoveBook", title);
        return _service.RemoveBook(title);
    }

    /*
subscription {
  onUpdateBook {
    title
    author {
      name
    }
  }
}
     */
    public async Task<Book> UpdateBook(Book book, [Service] ITopicEventSender sender)
    {
        await sender.SendAsync("OnUpdateBook", book);
        return _service.UpdateBook(book);
    }
}

public class Subscription
{
    /*
subscription {
  onAddBook {
    title
    author {
      name
    }
  }
}
     */
    [Subscribe]
    [Topic("OnAddBook")]
    public Book? OnAddBook([EventMessage] Book book)
    {
        return book;
    }
    
    /*
subscription {
  onRemoveBook
}
     */
    [Subscribe]
    [Topic("OnRemoveBook")]
    public string OnRemoveBook([EventMessage] string title)
    {
        return title;
    }    
    
    /*
subscription {
  onUpdateBook {
    title
  }
}
     */
    [Subscribe]
    [Topic("OnUpdateBook")]
    public Book OnUpdateBook([EventMessage] Book book)
    {
        return book;
    }        
}