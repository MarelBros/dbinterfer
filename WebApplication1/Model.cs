using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

public class Product
{
    public int ProductID { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public decimal Price { get; set; }

    [Required]
    public string Code { get; set; }

    [Required]
    public int WarehouseID { get; set; }

    [JsonIgnore]
    public Warehouse Warehouse { get; set; }

    [JsonIgnore]
    public ICollection<Transaction> Transactions { get; set; }
}

public class Supplier
{
    public int SupplierID { get; set; }

    [Required]
    public string Name { get; set; }

    public string Address { get; set; }

    [Required]
    public string INN { get; set; }

    public string Phone { get; set; }

    [JsonIgnore]
    public ICollection<Transaction> Transactions { get; set; }
}

public class Warehouse
{
    public int WarehouseID { get; set; }

    [Required]
    public string Name { get; set; }

    [JsonIgnore]
    public ICollection<Product> Products { get; set; }
}

public class Transaction
{
    public int TransactionID { get; set; }

    [Required]
    public int ProductID { get; set; }

    public int? SupplierID { get; set; }

    [Required]
    public int Quantity { get; set; }

    [Required]
    public string TransactionType { get; set; } // приход / расход

    public DateTime TransactionDate { get; set; }

    [JsonIgnore]
    public Product Product { get; set; }

    [JsonIgnore]
    public Supplier Supplier { get; set; }
}

public class ProductCreateDto
{
    [Required]
    public string Name { get; set; }

    [Required]
    public decimal Price { get; set; }

    public string? Code { get; set; }

    [Required]
    public int WarehouseID { get; set; }

    [Required]
    public int SupplierID { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int InitialQuantity { get; set; }
}