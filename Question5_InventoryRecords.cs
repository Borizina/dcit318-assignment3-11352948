using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace InventoryRecordsQ5
{
    // b) Marker interface
    public interface IInventoryEntity
    {
        int Id { get; }
    }

    // a) Immutable record
    public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

    // c) Generic InventoryLogger
    public class InventoryLogger<T> where T : IInventoryEntity
    {
        private readonly List<T> _log = new();
        private readonly string _filePath;

        public InventoryLogger(string filePath)
        {
            _filePath = filePath;
        }

        public void Add(T item) => _log.Add(item);

        public List<T> GetAll() => new List<T>(_log);

        public void SaveToFile()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(_log, options);
                using var writer = new StreamWriter(_filePath, false);
                writer.Write(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SaveToFile] Error: {ex.Message}");
                throw;
            }
        }

        public void LoadFromFile()
        {
            try
            {
                if (!File.Exists(_filePath))
                    throw new FileNotFoundException("Log file not found.", _filePath);

                using var reader = new StreamReader(_filePath);
                var json = reader.ReadToEnd();
                var items = JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();

                _log.Clear();
                _log.AddRange(items);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LoadFromFile] Error: {ex.Message}");
                throw;
            }
        }
    }

    // f) Integration layer
    public class InventoryApp
    {
        private InventoryLogger<InventoryItem> _logger;

        public InventoryApp(string filePath)
        {
            _logger = new InventoryLogger<InventoryItem>(filePath);
        }

        public void SeedSampleData()
        {
            _logger.Add(new InventoryItem(1, "USB Flash Drive 32GB", 40, DateTime.Today));
            _logger.Add(new InventoryItem(2, "Notebook A5", 120, DateTime.Today.AddDays(-2)));
            _logger.Add(new InventoryItem(3, "Laser Printer Toner", 15, DateTime.Today.AddDays(-7)));
            _logger.Add(new InventoryItem(4, "Office Chair", 6, DateTime.Today.AddDays(-20)));
            _logger.Add(new InventoryItem(5, "Ethernet Cable 5m", 60, DateTime.Today.AddDays(-1)));
        }

        public void SaveData() => _logger.SaveToFile();
        public void LoadData() => _logger.LoadFromFile();

        public void PrintAllItems()
        {
            Console.WriteLine("== Inventory Items ==");
            foreach (var item in _logger.GetAll())
            {
                Console.WriteLine($"Id={item.Id}, Name={item.Name}, Qty={item.Quantity}, DateAdded={item.DateAdded:yyyy-MM-dd}");
            }
        }

        public static void Main()
        {
            string path = "inventory_log.json";

            var app = new InventoryApp(path);
            app.SeedSampleData();
            app.SaveData();

            // Clear memory by recreating logger (simulate new session)
            app = new InventoryApp(path);
            app.LoadData();
            app.PrintAllItems();
        }
    }
}