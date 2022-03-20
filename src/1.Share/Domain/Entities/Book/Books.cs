using System;
using System.ComponentModel;

namespace Domain.Entities.Book;

public record Books
    (string ID, string ISBN, string WRITER, DateTime PUBLISH_DATE, [DefaultValue(false)] bool IS_EXPIRED = false) : NoSqlEntityBase(ID);