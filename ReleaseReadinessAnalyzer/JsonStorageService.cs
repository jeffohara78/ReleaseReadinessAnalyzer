using System.Text.Json;

namespace ReleaseReadinessAnalyzer.Services;

public class JsonStorageService<T>
{
    private readonly string _filePath;

    public JsonStorageService(string filePath)
    {
        _filePath = filePath;
    }

    public List<T> Load()
    {
        if (!File.Exists(_filePath))
        {
            return new List<T>();
        }

        string json = File.ReadAllText(_filePath);

        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<T>();
        }

        return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
    }

    public void Save(List<T> items)
    {
        string json = JsonSerializer.Serialize(items, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(_filePath, json);
    }
}