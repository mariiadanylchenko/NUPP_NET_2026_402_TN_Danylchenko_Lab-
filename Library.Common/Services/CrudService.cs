using System.Text.Json;
using Library.Common.Models;

namespace Library.Common.Services;

// generic CRUD сервіс — зберігає дані у Dictionary<Guid, T>
public class CrudService<T> : ICrudService<T> where T : class
{
    // внутрішня колекція — словник для O(1) доступу за Id
    private readonly Dictionary<Guid, T> _storage = new();

    // делегат для отримання Id з об'єкта
    private readonly Func<T, Guid> _idSelector;

    public CrudService(Func<T, Guid> idSelector)
    {
        _idSelector = idSelector;
    }

    // додати новий елемент
    public void Create(T element)
    {
        var id = _idSelector(element);
        if (_storage.ContainsKey(id))
            throw new InvalidOperationException($"Елемент з Id={id} вже існує.");
        _storage[id] = element;
    }

    // знайти елемент за Id
    public T Read(Guid id)
    {
        if (!_storage.TryGetValue(id, out var element))
            throw new KeyNotFoundException($"Елемент з Id={id} не знайдено.");
        return element;
    }

    // отримати всі елементи
    public IEnumerable<T> ReadAll() => _storage.Values;

    // оновити існуючий елемент
    public void Update(T element)
    {
        var id = _idSelector(element);
        if (!_storage.ContainsKey(id))
            throw new KeyNotFoundException($"Елемент з Id={id} не знайдено.");
        _storage[id] = element;
    }

    // видалити елемент
    public void Remove(T element)
    {
        var id = _idSelector(element);
        if (!_storage.Remove(id))
            throw new KeyNotFoundException($"Елемент з Id={id} не знайдено.");
    }

    // зберегти колекцію у JSON файл
    public void Save(string filePath)
    {
        var json = JsonSerializer.Serialize(_storage.Values, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
        Console.WriteLine($"Збережено {_storage.Count} записів у файл: {filePath}");
    }

    // завантажити колекцію з JSON файлу
    public void Load(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Файл не знайдено: {filePath}");

        var json = File.ReadAllText(filePath);
        var items = JsonSerializer.Deserialize<List<T>>(json);
        if (items is null) return;

        _storage.Clear();
        foreach (var item in items)
            _storage[_idSelector(item)] = item;

        Console.WriteLine($"Завантажено {_storage.Count} записів з файлу: {filePath}");
    }
}
