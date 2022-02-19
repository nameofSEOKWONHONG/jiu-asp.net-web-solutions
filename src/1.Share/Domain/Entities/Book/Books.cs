using System;
using System.ComponentModel;

namespace Domain.Entities.Book;

public class Books : NoSqlEntityBase
{
    public string ISBN { get; set; }
    public string WRITER { get; set; }
    public DateTime PUBLISH_DATE { get; set; }
    [DefaultValue(false)]
    public  bool IS_EXPIRED { get; set; }
}