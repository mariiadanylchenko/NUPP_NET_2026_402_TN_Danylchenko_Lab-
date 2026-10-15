using Library.Common.Extensions;
using Library.Common.Models;
using Library.Common.Services;

Console.OutputEncoding = System.Text.Encoding.UTF8;

// ============================================================
// ЛАБ 1 — БАЗОВА ДЕМОНСТРАЦІЯ
// ============================================================
Console.WriteLine("=== Лабораторна робота №1: Бібліотечна система ===\n");

var fiction  = new Genre("Фантастика", "Наукова фантастика та фентезі");
var detective = new Genre("Детектив", "Детективні та кримінальні романи");

var author1 = new Author("Іван", "Франко", "franko@lib.ua", "Каменяр", 1856, "Видатний поет.");
var author2 = new Author("Ліна", "Костенко", "kostenko@lib.ua", "ЛК", 1930, "Сучасна поетеса.");

// підписка на подію публікації
author1.OnBookPublished += (a, t) => Console.WriteLine($"  [ПОДІЯ] {a.GetFullName()} → \"{t}\"");
author2.OnBookPublished += (a, t) => Console.WriteLine($"  [ПОДІЯ] {a.GetFullName()} → \"{t}\"");

author1.PublishBook("Захар Беркут");
author2.PublishBook("Маруся Чурай");

var reader1 = new Reader("Марія", "Шевченко", "maria@mail.ua", "LIB-001", DateTime.Now);
reader1.OnBorrowLimitExceeded += (r, l) =>
    Console.WriteLine($"  [ПОПЕРЕДЖЕННЯ] {r.GetFullName()} перевищив ліміт {l} книг!");

var book1 = new Book("Захар Беркут", "978-966-01-0001", 1883, 150.00, author1.Id, fiction.Id);
var book2 = new Book("Маруся Чурай", "978-966-01-0002", 1979, 180.50, author2.Id, detective.Id);
var book3 = new Book("Чорна рада",   "978-966-01-0003", 1857, 120.00, author1.Id, fiction.Id);
var book4 = new Book("Перехресні стежки", "978-966-01-0004", 2015, 210.00, author2.Id, detective.Id);

// метод розширення
Console.WriteLine($"\nКнига \"{book3.Title}\" сучасна? {book3.IsModern()}");
Console.WriteLine($"Книга \"{book4.Title}\" сучасна? {book4.IsModern()}");

var bookService = new CrudService<Book>(b => b.Id);
bookService.Create(book1);
bookService.Create(book2);
bookService.Create(book3);
bookService.Create(book4);
Console.WriteLine($"\nCRUD: додано {bookService.ReadAll().Count()} книг.");

bookService.Remove(book3);
Console.WriteLine($"CRUD: після видалення — {bookService.ReadAll().Count()} книг.");
bookService.Save("books_lab1.json");

// ============================================================
// ЛАБ 2 — ASYNC + PARALLEL + СИНХРОНІЗАЦІЯ
// ============================================================
Console.WriteLine("\n=== Лабораторна робота №2: Async, Parallel, Sync primitives ===\n");

var asyncService = new CrudServiceAsync<Book>(b => b.Id, "books_lab2.json");

// ---- 1. Parallel.For — паралельне створення 1000 книг ----
Console.WriteLine("--- Паралельне створення 1000 книг (Parallel.For) ---");
var sw = System.Diagnostics.Stopwatch.StartNew();

Parallel.For(0, 1000, _ =>
{
    var book = Book.CreateNew();
    asyncService.CreateAsync(book).Wait();
});

sw.Stop();
Console.WriteLine($"Створено: {asyncService.Count} книг за {sw.ElapsedMilliseconds} мс\n");

// ---- 2. LINQ — мін/макс/середнє для числових полів ----
Console.WriteLine("--- LINQ: статистика по числових полях ---");
var allBooks = asyncService.ToList();

var minPrice   = allBooks.Min(b => b.Price);
var maxPrice   = allBooks.Max(b => b.Price);
var avgPrice   = allBooks.Average(b => b.Price);
var minYear    = allBooks.Min(b => b.PublishedYear);
var maxYear    = allBooks.Max(b => b.PublishedYear);
var avgYear    = allBooks.Average(b => b.PublishedYear);

Console.WriteLine($"Ціна    → мін: {minPrice:F2} | макс: {maxPrice:F2} | сер: {avgPrice:F2}");
Console.WriteLine($"Рік     → мін: {minYear}  | макс: {maxYear}  | сер: {avgYear:F1}");

var modernBooks = allBooks.Where(b => b.IsModern()).Count();
Console.WriteLine($"Сучасних книг (після 2000р.): {modernBooks}\n");

// ---- 3. Пагінація ----
Console.WriteLine("--- Пагінація: сторінка 0, розмір 5 ---");
var page = asyncService.ReadAllAsync(0, 5).Result;
foreach (var b in page)
    Console.WriteLine($"  {b.ToDisplayString()}");

// ---- 4. Async збереження у файл ----
Console.WriteLine("\n--- Async збереження у файл ---");
await asyncService.SaveAsync();
Console.WriteLine($"Збережено {asyncService.Count} записів у books_lab2.json");

// ---- 5. Lock — примітив синхронізації ----
Console.WriteLine("\n--- Lock: захищений лічильник з кількома потоками ---");
int lockCounter = 0;
object lockObj = new object();

Parallel.For(0, 100, _ =>
{
    // lock гарантує що лише один потік змінює лічильник одночасно
    lock (lockObj) { lockCounter++; }
});
Console.WriteLine($"Lock лічильник (очікується 100): {lockCounter}");

// ---- 6. Semaphore — обмеження паралельного доступу ----
Console.WriteLine("\n--- Semaphore: обмеження до 3 одночасних операцій ---");
var semaphore = new SemaphoreSlim(3, 3);
var semTasks = Enumerable.Range(1, 6).Select(async i =>
{
    await semaphore.WaitAsync();
    try
    {
        Console.WriteLine($"  Операція {i} розпочата (потік {Thread.CurrentThread.ManagedThreadId})");
        await Task.Delay(50);
        Console.WriteLine($"  Операція {i} завершена");
    }
    finally { semaphore.Release(); }
}).ToArray();

await Task.WhenAll(semTasks);

// ---- 7. AutoResetEvent — сигнал між потоками ----
Console.WriteLine("\n--- AutoResetEvent: сигнал між потоками ---");
var autoEvent = new AutoResetEvent(false);
string sharedData = string.Empty;

// виробник
var producer = Task.Run(() =>
{
    sharedData = "Дані готові!";
    Console.WriteLine("  Виробник: підготував дані, надсилає сигнал...");
    autoEvent.Set(); // надсилає сигнал
});

// споживач
var consumer = Task.Run(() =>
{
    autoEvent.WaitOne(); // чекає на сигнал
    Console.WriteLine($"  Споживач: отримав сигнал → \"{sharedData}\"");
});

await Task.WhenAll(producer, consumer);

Console.WriteLine("\n=== Кінець програми ===");
