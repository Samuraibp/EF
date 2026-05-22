using ConsoleApp5.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;

class Program
{
    static DbContextOptions<ApplicationContext> options;
    static bool isConnected = false;

    static void Main()
    {
        var builder = new ConfigurationBuilder();

        builder.SetBasePath(Directory.GetCurrentDirectory());
        builder.AddJsonFile("appsettings.json");

        var config = builder.Build();
        var connectionString = config.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
        options = optionsBuilder.UseSqlServer(connectionString).Options;

        bool running = true;

        while (running)
        {
            Console.WriteLine("\n=== MENU ===");

            Console.WriteLine("1. Connect to database");
            Console.WriteLine("2. Disconnect from database");

            Console.WriteLine("\n--- DATA ---");
            Console.WriteLine("3. Show all products");
            Console.WriteLine("4. Show product types");
            Console.WriteLine("5. Show suppliers");

            Console.WriteLine("\n--- STATISTICS ---");
            Console.WriteLine("6. Show statistics (min/max/avg)");

            Console.WriteLine("\n--- FILTERS ---");
            Console.WriteLine("7. Show products by type");
            Console.WriteLine("8. Show products by supplier");

            Console.WriteLine("\n--- SPECIAL ---");
            Console.WriteLine("9. Show oldest product");

            Console.WriteLine("0. Exit");

            Console.Write("Choose option: ");
            string choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        Connect();
                        break;

                    case "2":
                        Disconnect();
                        break;

                    case "3":
                        ShowProducts();
                        break;

                    case "4":
                        ShowTypes();
                        break;

                    case "5":
                        ShowSuppliers();
                        break;

                    case "6":
                        ShowStatistics();
                        break;

                    case "7":
                        ShowProductsByType();
                        break;

                    case "8":
                        ShowProductsBySupplier();
                        break;

                    case "9":
                        ShowOldestProduct();
                        break;

                    case "0":
                        running = false;
                        break;

                    default:
                        Console.WriteLine("Invalid option!");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }
        }
    }


    static void Connect()
    {
        if (isConnected)
        {
            Console.WriteLine("Already connected.");
            return;
        }

        isConnected = true;
        Console.WriteLine("Connected to database.");
    }

    static void Disconnect()
    {
        if (!isConnected)
        {
            Console.WriteLine("Already disconnected.");
            return;
        }

        isConnected = false;
        Console.WriteLine("Disconnected from database.");
    }


    static bool EnsureConnected()
    {
        if (!isConnected)
        {
            Console.WriteLine("ERROR: You are not connected to database.");
            return false;
        }
        return true;
    }


    static void ShowProducts()
    {
        if (!EnsureConnected()) return;

        using (var context = new ApplicationContext(options))
        {
            Console.WriteLine("\n=== ALL PRODUCTS ===");

            foreach (var p in context.Products)
            {
                Console.WriteLine($"{p.Name} | {p.Type} | {p.Supplier} | {p.Quantity} | {p.CostPrice} | {p.SupplyDate}");
            }
        }
    }

    static void ShowTypes()
    {
        if (!EnsureConnected()) return;

        using (var context = new ApplicationContext(options))
        {
            Console.WriteLine("\n=== PRODUCT TYPES ===");

            foreach (var t in context.Products.Select(p => p.Type).Distinct())
            {
                Console.WriteLine(t);
            }
        }
    }

    static void ShowSuppliers()
    {
        if (!EnsureConnected()) return;

        using (var context = new ApplicationContext(options))
        {
            Console.WriteLine("\n=== SUPPLIERS ===");

            foreach (var s in context.Products.Select(p => p.Supplier).Distinct())
            {
                Console.WriteLine(s);
            }
        }
    }

    static void ShowStatistics()
    {
        if (!EnsureConnected()) return;

        using (var context = new ApplicationContext(options))
        {
            var maxQty = context.Products.OrderByDescending(p => p.Quantity).First();
            var minQty = context.Products.OrderBy(p => p.Quantity).First();

            var maxCost = context.Products.OrderByDescending(p => p.CostPrice).First();
            var minCost = context.Products.OrderBy(p => p.CostPrice).First();

            Console.WriteLine("\n=== MAX QUANTITY ===");
            Console.WriteLine($"{maxQty.Name} - {maxQty.Quantity}");

            Console.WriteLine("\n=== MIN QUANTITY ===");
            Console.WriteLine($"{minQty.Name} - {minQty.Quantity}");

            Console.WriteLine("\n=== MAX COST ===");
            Console.WriteLine($"{maxCost.Name} - {maxCost.CostPrice}");

            Console.WriteLine("\n=== MIN COST ===");
            Console.WriteLine($"{minCost.Name} - {minCost.CostPrice}");

            Console.WriteLine("\n=== AVERAGE QUANTITY BY TYPE ===");

            var avg = context.Products
                .GroupBy(p => p.Type)
                .Select(g => new
                {
                    Type = g.Key,
                    AvgQuantity = g.Average(x => x.Quantity)
                });

            foreach (var a in avg)
            {
                Console.WriteLine($"{a.Type} - {a.AvgQuantity}");
            }
        }
    }

    static void ShowProductsByType()
    {
        if (!EnsureConnected()) return;

        using (var context = new ApplicationContext(options))
        {
            Console.Write("Enter product type: ");
            string type = Console.ReadLine();

            Console.WriteLine($"\n=== PRODUCTS OF TYPE {type} ===");

            var result = context.Products.Where(p => p.Type == type);

            foreach (var p in result)
            {
                Console.WriteLine($"{p.Name} | {p.Quantity} | {p.CostPrice}");
            }
        }
    }

    static void ShowProductsBySupplier()
    {
        if (!EnsureConnected()) return;

        using (var context = new ApplicationContext(options))
        {
            Console.Write("Enter supplier: ");
            string supplier = Console.ReadLine();

            Console.WriteLine($"\n=== PRODUCTS FROM SUPPLIER {supplier} ===");

            var result = context.Products.Where(p => p.Supplier == supplier);

            foreach (var p in result)
            {
                Console.WriteLine($"{p.Name} | {p.Quantity} | {p.CostPrice}");
            }
        }
    }

    static void ShowOldestProduct()
    {
        if (!EnsureConnected()) return;

        using (var context = new ApplicationContext(options))
        {
            var oldest = context.Products.OrderBy(p => p.SupplyDate).First();

            Console.WriteLine("\n=== OLDEST PRODUCT ===");
            Console.WriteLine($"{oldest.Name} - {oldest.SupplyDate}");
        }
    }
}