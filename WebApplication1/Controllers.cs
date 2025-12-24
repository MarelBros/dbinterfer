using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProductsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        var products = await _context.Products
            .Select(p => new
            {
                p.ProductID,
                p.Name,
                p.Price,
                p.Code,
                Quantity = _context.Transactions
                    .Where(t => t.ProductID == p.ProductID)
                    .Sum(t => t.TransactionType == "приход"
                        ? t.Quantity
                        : -t.Quantity)
            })
            .ToListAsync();

        return Ok(products);
    }
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] ProductCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        bool supplierExists = await _context.Suppliers
            .AnyAsync(s => s.SupplierID == dto.SupplierID);

        bool warehouseExists = await _context.Warehouses
            .AnyAsync(w => w.WarehouseID == dto.WarehouseID);

        if (!supplierExists || !warehouseExists)
            return BadRequest("Supplier or Warehouse not found");

        using var dbTransaction = await _context.Database.BeginTransactionAsync();
        var code = string.IsNullOrWhiteSpace(dto.Code)
            ? $"P-{Guid.NewGuid().ToString("N")[..10].ToUpper()}"
            : dto.Code;

        var product = new Product
        {
            Name = dto.Name,
            Price = dto.Price,
            Code = code
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var firstTransaction = new Transaction
        {
            ProductID = product.ProductID,
            WarehouseID = dto.WarehouseID,
            SupplierID = dto.SupplierID,
            Quantity = dto.InitialQuantity,
            TransactionType = "приход",
            TransactionDate = DateTime.UtcNow
        };

        _context.Transactions.Add(firstTransaction);
        await _context.SaveChangesAsync();

        await dbTransaction.CommitAsync();

        return CreatedAtAction(nameof(GetAllProducts), new
        {
            product.ProductID,
            product.Name,
            product.Price,
            product.Code,
            Quantity = dto.InitialQuantity
        });
    }
}
