using Library.Common.Models;
using Library.Common.Services;

namespace Library.Tests;

// Unit тести для CrudServiceAsync<T>
public class CrudServiceAsyncTests
{
    // допоміжний метод — сервіс із тимчасовим файлом
    private CrudServiceAsync<Book> CreateService(string? path = null) =>
        new(b => b.Id, path ?? Path.GetTempFileName());

    private static Book MakeBook(string title = "Test", double price = 100.0) =>
        new("978-000-0001", title, 2020, price, Guid.NewGuid(), Guid.NewGuid())
        { Title = title };

    // CreateAsync додає елемент успішно
    [Fact]
    public async Task CreateAsync_ShouldAddElement_ReturnTrue()
    {
        var svc = CreateService();
        var book = Book.CreateNew();

        var result = await svc.CreateAsync(book);

        Assert.True(result);
        Assert.Equal(1, svc.Count);
    }

    // CreateAsync двічі з одним Id повертає false
    [Fact]
    public async Task CreateAsync_DuplicateId_ReturnFalse()
    {
        var svc = CreateService();
        var book = Book.CreateNew();

        await svc.CreateAsync(book);
        var result = await svc.CreateAsync(book);

        Assert.False(result);
        Assert.Equal(1, svc.Count);
    }

    // ReadAsync повертає доданий елемент
    [Fact]
    public async Task ReadAsync_ExistingId_ReturnsElement()
    {
        var svc = CreateService();
        var book = Book.CreateNew();
        await svc.CreateAsync(book);

        var found = await svc.ReadAsync(book.Id);

        Assert.Equal(book.Id, found.Id);
    }

    // ReadAsync з неіснуючим Id кидає KeyNotFoundException
    [Fact]
    public async Task ReadAsync_NonExistingId_ThrowsKeyNotFoundException()
    {
        var svc = CreateService();

        await Assert.ThrowsAsync<KeyNotFoundException>(() => svc.ReadAsync(Guid.NewGuid()));
    }

    // ReadAllAsync повертає всі елементи
    [Fact]
    public async Task ReadAllAsync_ReturnsAllElements()
    {
        var svc = CreateService();
        await svc.CreateAsync(Book.CreateNew());
        await svc.CreateAsync(Book.CreateNew());
        await svc.CreateAsync(Book.CreateNew());

        var all = await svc.ReadAllAsync();

        Assert.Equal(3, all.Count());
    }

    // ReadAllAsync з пагінацією повертає правильну кількість
    [Fact]
    public async Task ReadAllAsync_Pagination_ReturnsCorrectPage()
    {
        var svc = CreateService();
        for (int i = 0; i < 10; i++)
            await svc.CreateAsync(Book.CreateNew());

        var page = await svc.ReadAllAsync(0, 3);

        Assert.Equal(3, page.Count());
    }

    // UpdateAsync оновлює існуючий елемент
    [Fact]
    public async Task UpdateAsync_ExistingElement_ReturnsTrueAndUpdates()
    {
        var svc = CreateService();
        var book = Book.CreateNew();
        await svc.CreateAsync(book);
        book.Price = 999.99;

        var result = await svc.UpdateAsync(book);
        var updated = await svc.ReadAsync(book.Id);

        Assert.True(result);
        Assert.Equal(999.99, updated.Price);
    }

    // UpdateAsync для неіснуючого елемента повертає false
    [Fact]
    public async Task UpdateAsync_NonExistingElement_ReturnsFalse()
    {
        var svc = CreateService();
        var book = Book.CreateNew();

        var result = await svc.UpdateAsync(book);

        Assert.False(result);
    }

    // RemoveAsync видаляє елемент
    [Fact]
    public async Task RemoveAsync_ExistingElement_ReturnsTrueAndRemoves()
    {
        var svc = CreateService();
        var book = Book.CreateNew();
        await svc.CreateAsync(book);

        var result = await svc.RemoveAsync(book);

        Assert.True(result);
        Assert.Equal(0, svc.Count);
    }

    // RemoveAsync для неіснуючого елемента повертає false
    [Fact]
    public async Task RemoveAsync_NonExistingElement_ReturnsFalse()
    {
        var svc = CreateService();
        var book = Book.CreateNew();

        var result = await svc.RemoveAsync(book);

        Assert.False(result);
    }

    // SaveAsync зберігає файл
    [Fact]
    public async Task SaveAsync_CreatesFile()
    {
        var path = Path.GetTempFileName();
        var svc = CreateService(path);
        await svc.CreateAsync(Book.CreateNew());

        var result = await svc.SaveAsync();

        Assert.True(result);
        Assert.True(File.Exists(path));
    }

    // IEnumerable<T> ітерується коректно
    [Fact]
    public async Task IEnumerable_IteratesAllElements()
    {
        var svc = CreateService();
        for (int i = 0; i < 5; i++)
            await svc.CreateAsync(Book.CreateNew());

        var count = svc.Count(b => b.Price >= 0);

        Assert.Equal(5, count);
    }

    // thread-safe: 500 паралельних CreateAsync без дублікатів
    [Fact]
    public async Task CreateAsync_Parallel500_AllAdded()
    {
        var svc = CreateService();
        var books = Enumerable.Range(0, 500).Select(_ => Book.CreateNew()).ToList();

        await Task.WhenAll(books.Select(b => svc.CreateAsync(b)));

        Assert.Equal(500, svc.Count);
    }
}
