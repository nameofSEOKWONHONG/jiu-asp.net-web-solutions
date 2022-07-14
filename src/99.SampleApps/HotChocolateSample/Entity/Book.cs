using Microsoft.AspNetCore.Server.Kestrel.Transport.Quic;

namespace HotChocolateSample.Entity;

public class Book
{
    public string Title { get; set; }

    public Author Author { get; set; }
}

public class Author
{
    public string Name { get; set; }
}

