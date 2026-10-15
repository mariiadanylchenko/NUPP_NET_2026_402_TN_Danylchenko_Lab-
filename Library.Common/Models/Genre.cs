namespace Library.Common.Models;

// клас жанру книги
public class Genre
{
    // статичне поле — список усіх доступних жанрів
    private static readonly List<string> _availableGenres = new()
    {
        "Фантастика", "Детектив", "Роман", "Поезія", "Наука", "Пригоди", "Жахи"
    };

    // статичний конструктор
    static Genre()
    {
        Console.WriteLine($"Доступно жанрів: {_availableGenres.Count}");
    }

    // конструктор з параметрами
    public Genre(string name, string description)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
    }

    // конструктор без параметрів
    public Genre() { Id = Guid.NewGuid(); }

    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // статичний метод — список доступних жанрів
    public static IReadOnlyList<string> GetAvailableGenres() => _availableGenres;

    // метод — опис жанру
    public string GetInfo() => $"{Name}: {Description}";
}
