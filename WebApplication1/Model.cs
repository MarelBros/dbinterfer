using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class Product
{
    public int ProductID { get; set; }
    public string Name { get; set; }
    public int WarehouseID { get; set; }
    public int SupplierID { get; set; }
    public int Quantity { get; set; }
    public string ProductCode { get; set; }

    [JsonIgnore]
    public Warehouse Warehouse { get; set; }
    [JsonIgnore]
    public Supplier Supplier { get; set; }

    [JsonIgnore]
    public ICollection<Transaction> Transactions { get; set; }

}

public class ProductCreate
{
    public string Name { get; set; }
    public int WarehouseID { get; set; }
    public int SupplierID { get; set; }
    public int Quantity { get; set; }
    public string ProductCode { get; set; }
}

public class Supplier
{
    public int SupplierID { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string INN { get; set; }
    public string Phone { get; set; }

    [JsonIgnore]
    public ICollection<Product> Products { get; set; }
}
public class Warehouse
{
    public int WarehouseID { get; set; }
    public string Name { get; set; }

    [JsonIgnore]
    public ICollection<Product> Products { get; set; }
}

public class Transaction
{
    public int TransactionID { get; set; }
    public int ProductID { get; set; }
    public int Quantity { get; set; }
    public decimal Amount { get; set; }
    public string TransactionType { get; set; }
    public DateTime TransactionDate { get; set; }

    [JsonIgnore]
    public Product Product { get; set; }
}
