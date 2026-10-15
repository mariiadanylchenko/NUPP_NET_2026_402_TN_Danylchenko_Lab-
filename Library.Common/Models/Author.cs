namespace Library.Common.Models;

// делегат для події публікації нової книги автором
public delegate void BookPublishedHandler(Author author, string bookTitle);

// клас автора — успадковує Person
public class Author : Person
{
    // статичне поле — кількість авторів у системі
    private static int _authorCount = 0;

    // статичний конструктор
    static Author()
    {
        _authorCount = 0;
    }

    // конструктор з параметрами
    public Author(string firstName, string lastName, string email,
                  string pseudonym, int birthYear, string biography)
        : base(firstName, lastName, email)
    {
        Pseudonym = pseudonym;
        BirthYear = birthYear;
        Biography = biography;
        _authorCount++;
    }

    // конструктор без параметрів
    public Author() { }

    public string Pseudonym { get; set; } = string.Empty;
    public int BirthYear { get; set; }
    public string Biography { get; set; } = string.Empty;

    // подія — спрацьовує коли автор публікує книгу
    public event BookPublishedHandler? OnBookPublished;

    // метод — автор публікує книгу (викликає подію)
    public void PublishBook(string bookTitle)
    {
        Console.WriteLine($"Автор {GetFullName()} публікує книгу: {bookTitle}");
        OnBookPublished?.Invoke(this, bookTitle);
    }

    // статичний метод — повертає кількість авторів
    public static int GetAuthorCount() => _authorCount;

    // переозначення методу опису
    public override string GetDescription() =>
        $"Автор: {GetFullName()}, псевдонім: {Pseudonym}, рік народження: {BirthYear}";
}
