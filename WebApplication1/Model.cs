using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Product
{
    [Key]
    public int ProductID { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; }

    [ForeignKey(nameof(Warehouse))]
    public int WarehouseID { get; set; }

    public int Quantity { get; set; }

    [Required]
    [MaxLength(50)]
    public string ProductCode { get; set; }

    [ForeignKey(nameof(Supplier))]
    public int SupplierID { get; set; }

    public Warehouse Warehouse { get; set; }
    public Supplier Supplier { get; set; }

    public ICollection<Transaction> Transactions { get; set; }
}

public class Supplier
{
    [Key]
    public int SupplierID { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; }

    [MaxLength(255)]
    public string Address { get; set; }

    [Required]
    [MaxLength(12)]
    public string INN { get; set; }

    [MaxLength(20)]
    public string Phone { get; set; }

    public ICollection<Product> Products { get; set; }
}

public class Warehouse
{
    [Key]
    public int WarehouseID { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    public ICollection<Product> Products { get; set; }
}

public class Transaction
{
    [Key]
    public int TransactionID { get; set; }

    [ForeignKey(nameof(Product))]
    public int ProductID { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "money")]
    public decimal Amount { get; set; }

    [Required]
    [MaxLength(10)]
    public string TransactionType { get; set; }

    public DateTime TransactionDate { get; set; }

    public Product Product { get; set; }
}
