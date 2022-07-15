using eXtensionSharp;
using HotChocolateSample.Entity;

namespace HotChocolateSample.GraphQL;

/*
 * if use ef, this code don't impl.
 * impl to query, mutation.
 */
public class BookService
{
    private List<Book> _books;

    public BookService()
    {
        _books = new();
        _books.Add(new Book()
        {
            Title = "C# in depth.",
            Author = new Author
            {
                Name = "Jon Skeet"
            }
        });
    }

    public IEnumerable<Book> GetBooks() => _books;
    public Book? GetBookByTitle(string title) => _books.FirstOrDefault(m => m.Title == title);
    public Book? GetBookByName(string name) => _books.FirstOrDefault(m => m.Author.Name == name);

    public Book? AddBook(Book item)
    {
        var exists = this.GetBookByTitle(item.Title);
        if (exists is not null) return null;
        _books.Add(item);
        return item;
    }

    public bool RemoveBook(string title)
    {
        var exists = this.GetBookByTitle(title);
        if (exists is null) return false;
        _books.Remove(exists);
        return true;
    }

    public Book UpdateBook(Book book)
    {
        var exists = this.GetBookByTitle(book.Title);
        if (exists.xIsEmpty()) return null;
        exists.Author.Name = book.Author.Name;
        return exists;
    }
}