
namespace PayrollApp.Services;

using System.Text.Json;

public class JsonStorage
{
    private readonly string _folder;
    private readonly object _lock = new();
    private readonly JsonSerializerOptions _opts = new() { WriteIndented = true };

    public JsonStorage(string folder)
    {
        _folder = folder;
    }

    private string PathFor(string file) => System.IO.Path.Combine(_folder, file);

    public List<T> GetAll<T>(string file)
    {
        lock (_lock)
        {
            var p = PathFor(file);
            if (!File.Exists(p)) return new List<T>();
            var txt = File.ReadAllText(p);
            return JsonSerializer.Deserialize<List<T>>(txt) ?? new List<T>();
        }
    }

    public T? Find<T>(string file, Guid id) where T : class
    {
        var list = GetAll<T>(file);
        return list.FirstOrDefault(item =>
        {
            var prop = item!.GetType().GetProperty("Id");
            if (prop is null) return false;
            var val = prop.GetValue(item);
            return val is Guid g && g == id;
        });
    }

    public void Add<T>(string file, T item)
    {
        lock (_lock)
        {
            var list = GetAll<T>(file);
            list.Add(item);
            File.WriteAllText(PathFor(file), JsonSerializer.Serialize(list, _opts));
        }
    }

    public bool Update<T>(string file, Guid id, T newItem)
    {
        lock (_lock)
        {
            var list = GetAll<T>(file);
            var ix = list.FindIndex(item =>
            {
                var prop = item!.GetType().GetProperty("Id");
                if (prop is null) return false;
                var val = prop.GetValue(item);
                return val is Guid g && g == id;
            });
            if (ix == -1) return false;
            var propNew = newItem!.GetType().GetProperty("Id");
            if (propNew is not null) propNew.SetValue(newItem, id);
            list[ix] = newItem;
            File.WriteAllText(PathFor(file), JsonSerializer.Serialize(list, _opts));
            return true;
        }
    }

    public bool Delete<T>(string file, Guid id)
    {
        lock (_lock)
        {
            var list = GetAll<T>(file);
            var newList = list.Where(item =>
            {
                var prop = item!.GetType().GetProperty("Id");
                if (prop is null) return true;
                var val = prop.GetValue(item);
                return !(val is Guid g && g == id);
            }).ToList();
            if (newList.Count == list.Count) return false;
            File.WriteAllText(PathFor(file), JsonSerializer.Serialize(newList, _opts));
            return true;
        }
    }
}
