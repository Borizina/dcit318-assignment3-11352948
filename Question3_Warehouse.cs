using System;
using System.Collections.Generic;

namespace WarehouseQ3
{
    // a) Marker interface
    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

    // b) ElectronicItem
    public class ElectronicItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public string Brand { get; }
        public int WarrantyMonths { get; }

        public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            Brand = brand;
            WarrantyMonths = warrantyMonths;
        }

        public override string ToString() => $"ElectronicItem {{ Id={Id}, Name={Name}, Brand={Brand}, Warranty={WarrantyMonths}m, Qty={Quantity} }}";
    }

    // c) GroceryItem
    public class GroceryItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; }

        public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            ExpiryDate = expiryDate;
        }

        public override string ToString() => $"GroceryItem {{ Id={Id}, Name={Name}, Expiry={ExpiryDate:yyyy-MM-dd}, Qty={Quantity} }}";
    }

    // e) Custom exceptions
    public class DuplicateItemException : Exception
    {
        public DuplicateItemException(string message) : base(message) { }
    }
    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string message) : base(message) { }
    }
    public class InvalidQuantityException : Exception
    {
        public InvalidQuantityException(string message) : base(message) { }
    }

    // d) InventoryRepository<T>
    public class InventoryRepository<T> where T : IInventoryItem
    {
        private readonly Dictionary<int, T> _items = new();

        public void AddItem(T item)
        {
            if (_items.ContainsKey(item.Id))
                throw new DuplicateItemException($"An item with Id {item.Id} already exists.");
            _items[item.Id] = item;
        }

        public T GetItemById(int id)
        {
            if (!_items.TryGetValue(id, out var item))
                throw new ItemNotFoundException($"No item found with Id {id}.");
            return item;
        }

        public void RemoveItem(int id)
        {
            if (!_items.Remove(id))
                throw new ItemNotFoundException($"No item found with Id {id} to remove.");
        }

        public List<T> GetAllItems() => new List<T>(_items.Values);

        public void UpdateQuantity(int id, int newQuantity)
        {
            if (newQuantity < 0)
                throw new InvalidQuantityException("Quantity cannot be negative.");
            var item = GetItemById(id);
            item.Quantity = newQuantity;
        }
    }

    // f) WareHouseManager
    public class WareHouseManager
    {
        private readonly InventoryRepository<ElectronicItem> _electronics = new();
        private readonly InventoryRepository<GroceryItem> _groceries = new();

        public void SeedData()
        {
            _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 24));
            _electronics.AddItem(new ElectronicItem(2, "Smartphone", 20, "Samsung", 12));

            _groceries.AddItem(new GroceryItem(100, "Rice (5kg)", 50, DateTime.Today.AddMonths(12)));
            _groceries.AddItem(new GroceryItem(101, "Milk", 30, DateTime.Today.AddDays(20)));
        }

        public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
        {
            foreach (var item in repo.GetAllItems())
                Console.WriteLine(item);
        }

        public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
        {
            try
            {
                var item = repo.GetItemById(id);
                var newQty = item.Quantity + quantity;
                if (newQty < 0) throw new InvalidQuantityException("Resulting quantity cannot be negative.");
                repo.UpdateQuantity(id, newQty);
                Console.WriteLine($"Updated Id {id} stock to {newQty}.");
            }
            catch (Exception ex) when (ex is ItemNotFoundException || ex is InvalidQuantityException)
            {
                Console.WriteLine($"[IncreaseStock] Error: {ex.Message}");
            }
        }

        public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
        {
            try
            {
                repo.RemoveItem(id);
                Console.WriteLine($"Removed item with Id {id}.");
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"[RemoveItem] Error: {ex.Message}");
            }
        }

        public static void Main()
        {
            var mgr = new WareHouseManager();
            try
            {
                mgr.SeedData();
            }
            catch (DuplicateItemException ex)
            {
                Console.WriteLine($"[SeedData] {ex.Message}");
            }

            Console.WriteLine("== Grocery Items ==");
            mgr.PrintAllItems(mgr._groceries);

            Console.WriteLine("\n== Electronic Items ==");
            mgr.PrintAllItems(mgr._electronics);

            // v) Trigger exceptions
            Console.WriteLine("\n== Triggering Exceptions ==");
            try
            {
                // Add duplicate
                mgr._electronics.AddItem(new ElectronicItem(1, "Tablet", 5, "Apple", 12));
            }
            catch (DuplicateItemException ex)
            {
                Console.WriteLine($"[Duplicate] {ex.Message}");
            }

            // Remove non-existent
            mgr.RemoveItemById(mgr._groceries, 999);

            // Update with invalid quantity
            try
            {
                mgr._groceries.UpdateQuantity(100, -5);
            }
            catch (InvalidQuantityException ex)
            {
                Console.WriteLine($"[InvalidQuantity] {ex.Message}");
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"[ItemNotFound] {ex.Message}");
            }

            // Happy path adjustments
            mgr.IncreaseStock(mgr._electronics, 2, 5);
            mgr.IncreaseStock(mgr._groceries, 101, -10);
        }
    }
}