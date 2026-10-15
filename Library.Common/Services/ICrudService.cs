namespace Library.Common.Services;

// інтерфейс generic CRUD сервісу
public interface ICrudService<T>
{
    public void Create(T element);
    public T Read(Guid id);
    public IEnumerable<T> ReadAll();
    public void Update(T element);
    public void Remove(T element);

    // додаткове завдання: збереження та завантаження з файлу
    public void Save(string filePath);
    public void Load(string filePath);
}
