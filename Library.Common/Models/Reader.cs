namespace Library.Common.Models;

// делегат для події перевищення ліміту книг
public delegate void BorrowLimitExceededHandler(Reader reader, int limit);

// клас читача — успадковує Person
public class Reader : Person
{
    // статичне поле — максимальна кількість книг для позики
    public static readonly int MaxBorrowLimit = 5;

    // конструктор з параметрами
    public Reader(string firstName, string lastName, string email,
                  string libraryCardNumber, DateTime membershipDate)
        : base(firstName, lastName, email)
    {
        LibraryCardNumber = libraryCardNumber;
        MembershipDate = membershipDate;
        BorrowedBooksCount = 0;
    }

    // конструктор без параметрів
    public Reader() { }

    public string LibraryCardNumber { get; set; } = string.Empty;
    public DateTime MembershipDate { get; set; }
    public int BorrowedBooksCount { get; set; }

    // подія — спрацьовує при перевищенні ліміту книг
    public event BorrowLimitExceededHandler? OnBorrowLimitExceeded;

    // метод — взяти книгу
    public void BorrowBook()
    {
        BorrowedBooksCount++;
        if (BorrowedBooksCount > MaxBorrowLimit)
            OnBorrowLimitExceeded?.Invoke(this, MaxBorrowLimit);
    }

    // метод — повернути книгу
    public void ReturnBook()
    {
        if (BorrowedBooksCount > 0)
            BorrowedBooksCount--;
    }

    // статичний метод — перевірка чи читач може взяти більше книг
    public static bool CanBorrow(Reader reader) =>
        reader.BorrowedBooksCount < MaxBorrowLimit;

    // переозначення методу опису
    public override string GetDescription() =>
        $"Читач: {GetFullName()}, квиток: {LibraryCardNumber}, книг на руках: {BorrowedBooksCount}/{MaxBorrowLimit}";
}
