using System.Collections.Concurrent;
using PeopleTeamsProjectsRepository.Repository;

namespace PeopleTeamsProjectsSample.Repository;

public sealed class InMemoryRepository<T> : IRepository<T> where T : class
{
    private readonly ConcurrentDictionary<Guid, T> _store = new();
    private static Guid GetId(T entity) => (Guid)(typeof(T).GetProperty("Id")!.GetValue(entity)!);
    
    public InMemoryRepository(IEnumerable<T> seed, bool overwrite = false) 
    {
        foreach (var item in seed)
        {
            var id = GetId(item);
            if (overwrite) _store[id] = item; else _store.TryAdd(id, item);
        }
    }

    public IEnumerable<T> GetAll() => _store.Values.OrderBy(GetId);
    public T? Get(Guid id)          => _store.GetValueOrDefault(id);
    public void Add(T entity)       => _store[GetId(entity)] = entity;
    public void Update(T entity)    => _store[GetId(entity)] = entity;
    public void Delete(Guid id)     => _store.TryRemove(id, out _);
    public T? GetByName(string name)
    {
        return GetAll().FirstOrDefault(e => 
            string.Equals(e.GetType().GetProperty("Name")?.GetValue(e)?.ToString(), name, StringComparison.OrdinalIgnoreCase));
    }
}
