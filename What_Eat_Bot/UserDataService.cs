using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class UserDataService
{
    // Файл, в котором будут храниться добавленные блюда
    private readonly string _filePath =
        Path.Combine(AppContext.BaseDirectory, "user_data.json");

    private Dictionary<string, List<string>> _dishes =
        new(StringComparer.OrdinalIgnoreCase);

    public UserDataService()
    {
        Load();
    }

    // Загружаем блюда из JSON
    private void Load()
    {
        if (!File.Exists(_filePath))
            return;

        try
        {
            string json = File.ReadAllText(_filePath);

            var loaded =
                JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json);

            if (loaded != null)
            {
                _dishes = new Dictionary<string, List<string>>(
                    loaded,
                    StringComparer.OrdinalIgnoreCase);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка чтения JSON: {ex.Message}");
        }
    }

    // Получаем блюда выбранной категории
    public List<string> GetDishes(string category)
    {
        if (_dishes.TryGetValue(category, out var dishes))
            return dishes;

        return new List<string>();
    }

    // Добавляем новое блюдо
    public bool AddDish(string category, string dish)
    {
        dish = dish.Trim();

        if (string.IsNullOrWhiteSpace(dish))
            return false;

        if (!_dishes.ContainsKey(category))
            _dishes[category] = new List<string>();

        // Проверяем, нет ли такого блюда в списке
        if (_dishes[category].Any(existing =>
            string.Equals(
                existing,
                dish,
                StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        _dishes[category].Add(dish);

        Save();

        return true;
    }

    // Сохраняем изменения в JSON
    private void Save()
    {
        string json = JsonSerializer.Serialize(
            _dishes,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        File.WriteAllText(_filePath, json);
    }
}