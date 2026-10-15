namespace Library.Common.Services;

// async generic CRUD інтерфейс з підтримкою пагінації та IEnumerable
public interface ICrudServiceAsync<T> : IEnumerable<T>
{
    public Task<bool> CreateAsync(T element);
    public Task<T> ReadAsync(Guid id);
    public Task<IEnumerable<T>> ReadAllAsync();

    // пагінація: page — номер сторінки (з 0), amount — кількість елементів
    public Task<IEnumerable<T>> ReadAllAsync(int page, int amount);
    public Task<bool> UpdateAsync(T element);
    public Task<bool> RemoveAsync(T element);

    // зберегти колекцію асинхронно у файл
    public Task<bool> SaveAsync();
}
