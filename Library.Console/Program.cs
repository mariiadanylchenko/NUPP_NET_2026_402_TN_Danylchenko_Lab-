using Library.Common.Extensions;
using Library.Common.Models;
using Library.Common.Services;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("=== Бібліотечна система — Лабораторна робота №1 ===\n");

// ---- Демонстрація моделей ----
Console.WriteLine("--- Створення жанрів ---");
var fiction = new Genre("Фантастика", "Наукова фантастика та фентезі");
var detective = new Genre("Детектив", "Детективні та кримінальні романи");
Console.WriteLine(fiction.GetInfo());
Console.WriteLine(detective.GetInfo());
Console.WriteLine($"Доступні жанри: {string.Join(", ", Genre.GetAvailableGenres())}\n");

Console.WriteLine("--- Створення авторів ---");
var author1 = new Author("Іван", "Франко", "franko@lib.ua", "Каменяр", 1856,
    "Видатний український письменник і поет.");
var author2 = new Author("Ліна", "Костенко", "kostenko@lib.ua", "ЛК", 1930,
    "Сучасна українська поетеса.");

// підписка на подію публікації книги
author1.OnBookPublished += (author, title) =>
    Console.WriteLine($"  [ПОДІЯ] Нова книга від {author.GetFullName()}: \"{title}\"");
author2.OnBookPublished += (author, title) =>
    Console.WriteLine($"  [ПОДІЯ] Нова книга від {author.GetFullName()}: \"{title}\"");

Console.WriteLine(author1.GetDescription());
Console.WriteLine(author2.GetDescription());
author1.PublishBook("Захар Беркут");
author2.PublishBook("Маруся Чурай");
Console.WriteLine($"Авторів у системі: {Author.GetAuthorCount()}\n");

Console.WriteLine("--- Створення читачів ---");
var reader1 = new Reader("Марія", "Шевченко", "maria@mail.ua", "LIB-001", DateTime.Now);
var reader2 = new Reader("Олег", "Кравченко", "oleg@mail.ua", "LIB-002", DateTime.Now.AddYears(-2));

// підписка на подію перевищення ліміту
reader1.OnBorrowLimitExceeded += (reader, limit) =>
    Console.WriteLine($"  [ПОПЕРЕДЖЕННЯ] {reader.GetFullName()} перевищив ліміт {limit} книг!");

Console.WriteLine(reader1.GetDescription());
Console.WriteLine(reader2.GetDescription());
Console.WriteLine($"Чи може {reader1.GetFullName()} взяти книгу: {Reader.CanBorrow(reader1)}\n");

Console.WriteLine("--- Створення книг ---");
var book1 = new Book("Захар Беркут", "978-966-01-0001", 1883, 150.00, author1.Id, fiction.Id);
var book2 = new Book("Маруся Чурай", "978-966-01-0002", 1979, 180.50, author2.Id, detective.Id);
var book3 = new Book("Чорна рада", "978-966-01-0003", 1857, 120.00, author1.Id, fiction.Id);
var book4 = new Book("Перехресні стежки", "978-966-01-0004", 2015, 210.00, author2.Id, detective.Id);

// метод розширення
Console.WriteLine(book1.ToDisplayString());
Console.WriteLine(book2.ToDisplayString());
Console.WriteLine($"Книга \"{book3.Title}\" сучасна? {book3.IsModern()}");
Console.WriteLine($"Книга \"{book4.Title}\" сучасна? {book4.IsModern()}");
Console.WriteLine($"Загальна кількість книг у системі: {Book.GetTotalBooks()}\n");

// ---- CRUD Сервіс ----
Console.WriteLine("=== Демонстрація CRUD сервісу ===\n");
var bookService = new CrudService<Book>(b => b.Id);

// Create
Console.WriteLine("--- Create ---");
bookService.Create(book1);
bookService.Create(book2);
bookService.Create(book3);
bookService.Create(book4);
Console.WriteLine("Додано 4 книги.\n");

// ReadAll
Console.WriteLine("--- ReadAll ---");
foreach (var b in bookService.ReadAll())
    Console.WriteLine($"  {b.ToDisplayString()}");
Console.WriteLine();

// Read
Console.WriteLine("--- Read (за Id) ---");
var found = bookService.Read(book2.Id);
Console.WriteLine($"Знайдено: {found.GetSummary()}\n");

// Update
Console.WriteLine("--- Update ---");
book1.Price = 175.00;
bookService.Update(book1);
Console.WriteLine($"Оновлено ціну книги \"{book1.Title}\" → {book1.Price} грн\n");

// Remove
Console.WriteLine("--- Remove ---");
bookService.Remove(book3);
Console.WriteLine($"Видалено: \"{book3.Title}\"");
Console.WriteLine($"Залишилось книг: {bookService.ReadAll().Count()}\n");

// Save / Load (додаткове завдання)
Console.WriteLine("--- Save / Load ---");
var savePath = "books_data.json";
bookService.Save(savePath);

var bookService2 = new CrudService<Book>(b => b.Id);
bookService2.Load(savePath);
Console.WriteLine("Книги після завантаження:");
foreach (var b in bookService2.ReadAll())
    Console.WriteLine($"  {b.ToDisplayString()}");

Console.WriteLine("\n=== Кінець демонстрації ===");
