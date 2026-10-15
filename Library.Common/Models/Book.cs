namespace Library.Common.Models;

// клас книги
public class Book
{
    // масиви для генерації випадкових даних у CreateNew()
    private static readonly string[] _titles = { "Тіні забутих предків", "Лісова пісня", "Кобзар", "Eneyida", "Чорна рада", "Місто", "Хіба ревуть воли", "Земля", "Fata morgana", "Вогнепальні речі" };
    private static readonly string[] _isbnPrefixes = { "978-966", "978-617", "978-0", "978-1" };

    // статичне поле — загальна кількість створених книг
    private static int _totalBooks = 0;

    // статичний конструктор
    static Book()
    {
        _totalBooks = 0;
    }

    // конструктор з параметрами
    public Book(string title, string isbn, int publishedYear, double price, Guid authorId, Guid genreId)
    {
        Id = Guid.NewGuid();
        Title = title;
        Isbn = isbn;
        PublishedYear = publishedYear;
        Price = price;
        AuthorId = authorId;
        GenreId = genreId;
        _totalBooks++;
    }

    // конструктор без параметрів
    public Book() { Id = Guid.NewGuid(); }

    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Isbn { get; set; } = string.Empty;
    public int PublishedYear { get; set; }
    public double Price { get; set; }
    public Guid AuthorId { get; set; }
    public Guid GenreId { get; set; }

    // статичний метод — створює книгу зі згенерованими даними (для Parallel тестів)
    public static Book CreateNew()
    {
        var rng = new Random();
        return new Book(
            _titles[rng.Next(_titles.Length)],
            $"{_isbnPrefixes[rng.Next(_isbnPrefixes.Length)]}-{rng.Next(1000, 9999)}",
            rng.Next(1900, 2026),
            Math.Round(rng.NextDouble() * 500 + 50, 2),
            Guid.NewGuid(),
            Guid.NewGuid()
        );
    }

    // статичний метод — повертає загальну кількість книг
    public static int GetTotalBooks() => _totalBooks;

    // метод — короткий опис книги
    public string GetSummary() => $"\"{Title}\" (ISBN: {Isbn}, {PublishedYear} р., {Price:F2} грн)";
}
