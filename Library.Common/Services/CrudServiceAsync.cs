using System.Collections.Concurrent;
using System.Collections;
using System.Text.Json;

namespace Library.Common.Services;

// thread-safe async CRUD сервіс на базі ConcurrentDictionary
public class CrudServiceAsync<T> : ICrudServiceAsync<T> where T : class
{
    // ConcurrentDictionary — потокобезпечна колекція .NET
    private readonly ConcurrentDictionary<Guid, T> _storage = new();
    private readonly Func<T, Guid> _idSelector;
    private readonly string _filePath;

    // SemaphoreSlim для обмеження одночасних операцій запису у файл
    private readonly SemaphoreSlim _fileSemaphore = new(1, 1);

    public CrudServiceAsync(Func<T, Guid> idSelector, string filePath = "data.json")
    {
        _idSelector = idSelector;
        _filePath = filePath;
    }

    // додати новий елемент асинхронно
    public Task<bool> CreateAsync(T element)
    {
        var id = _idSelector(element);
        var added = _storage.TryAdd(id, element);
        return Task.FromResult(added);
    }

    // знайти елемент за Id асинхронно
    public Task<T> ReadAsync(Guid id)
    {
        if (!_storage.TryGetValue(id, out var element))
            throw new KeyNotFoundException($"Елемент з Id={id} не знайдено.");
        return Task.FromResult(element);
    }

    // отримати всі елементи асинхронно
    public Task<IEnumerable<T>> ReadAllAsync() =>
        Task.FromResult<IEnumerable<T>>(_storage.Values.ToList());

    // пагінація: page — номер сторінки (з 0), amount — розмір сторінки
    public Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
    {
        var result = _storage.Values
            .Skip(page * amount)
            .Take(amount)
            .ToList();
        return Task.FromResult<IEnumerable<T>>(result);
    }

    // оновити елемент асинхронно
    public Task<bool> UpdateAsync(T element)
    {
        var id = _idSelector(element);
        if (!_storage.ContainsKey(id)) return Task.FromResult(false);
        _storage[id] = element;
        return Task.FromResult(true);
    }

    // видалити елемент асинхронно
    public Task<bool> RemoveAsync(T element)
    {
        var id = _idSelector(element);
        return Task.FromResult(_storage.TryRemove(id, out _));
    }

    // зберегти колекцію у файл асинхронно — захищено SemaphoreSlim
    public async Task<bool> SaveAsync()
    {
        // SemaphoreSlim гарантує що лише один потік пише у файл одночасно
        await _fileSemaphore.WaitAsync();
        try
        {
            var json = JsonSerializer.Serialize(_storage.Values.ToList(),
                new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_filePath, json);
            return true;
        }
        finally
        {
            _fileSemaphore.Release();
        }
    }

    // завантажити дані з файлу
    public async Task LoadAsync()
    {
        if (!File.Exists(_filePath)) return;
        var json = await File.ReadAllTextAsync(_filePath);
        var items = JsonSerializer.Deserialize<List<T>>(json);
        if (items is null) return;
        _storage.Clear();
        foreach (var item in items)
            _storage.TryAdd(_idSelector(item), item);
    }

    // IEnumerable<T> — ітерація по колекції
    public IEnumerator<T> GetEnumerator() => _storage.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => _storage.Count;
}
