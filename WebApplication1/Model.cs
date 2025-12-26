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
    public string Name { get; set; } = null!;

    public string? Address { get; set; }   

    [Required]
    public string INN { get; set; } = null!;

    public string? Phone { get; set; }    

    [JsonIgnore]
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}

public class Warehouse
{
    public int WarehouseID { get; set; }

    [Required]
    public string Name { get; set; }

    [JsonIgnore]
    public ICollection<Product> Products { get; set; }
}

public class WarehouseDto
{
    [Required]
    public string Name { get; set; }
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

public class TransactionDto
{
    [Required]
    public int ProductID { get; set; }

    public int? SupplierID { get; set; }

    [Required]
    public int Quantity { get; set; }

    [Required]
    public string TransactionType { get; set; } // "приход" / "расход"
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

public class User
{
    public int UserID { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; }

    [Required]
    [MaxLength(255)]
    public string Password { get; set; } 

    [Required]
    [Range(0, 1)]
    public int Tier { get; set; }
}
public class LoginRequestDto
{
    public string Name { get; set; }
    public string Password { get; set; }
}

public class SupplierDto
{
    public string Name { get; set; } = null!;
    public string Inn { get; set; } = null!;
    public string? Phone { get; set; }
}


public class LoginResponseDto
{
    public string Name { get; set; }
    public int Tier { get; set; } // 0 - пользователь, 1 - админ
}