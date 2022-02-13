using System;

namespace Domain.Entities.Book;

public class Books : NoSqlEntityBase
{
    public string ISBN { get; set; }
    public string WRITER { get; set; }
    public DateTime PUBLISH_DATE { get; set; }
}