using Library.Common.Models;

namespace Library.Common.Extensions;

// методи розширення для класу Book
public static class BookExtensions
{
    // метод розширення — перевіряє чи книга є сучасною (після 2000 р.)
    public static bool IsModern(this Book book) => book.PublishedYear >= 2000;

    // метод розширення — форматований вивід книги
    public static string ToDisplayString(this Book book) =>
        $"[{book.PublishedYear}] {book.Title} | ISBN: {book.Isbn} | Ціна: {book.Price:F2} грн";
}
