namespace Library.Common.Models;

// базовий клас для людини (автор і читач успадковують від нього)
public abstract class Person
{
    // статичне поле — лічильник створених людей
    private static int _personCount = 0;

    // статичний конструктор — ініціалізація статичних полів
    static Person()
    {
        _personCount = 0;
    }

    // конструктор з параметрами
    public Person(string firstName, string lastName, string email)
    {
        Id = Guid.NewGuid();
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        _personCount++;
    }

    // конструктор без параметрів
    public Person()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // статичний метод — повертає кількість створених людей
    public static int GetPersonCount() => _personCount;

    // метод — повне ім'я
    public string GetFullName() => $"{FirstName} {LastName}";

    // віртуальний метод для опису — переозначається у нащадках
    public virtual string GetDescription() => $"{GetFullName()} ({Email})";
}
