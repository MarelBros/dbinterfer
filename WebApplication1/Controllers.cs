using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/products")]
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
                Warehouse = p.Warehouse.Name,
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

        if (!await _context.Warehouses.AnyAsync(w => w.WarehouseID == dto.WarehouseID))
            return BadRequest("Warehouse not found");

        if (!await _context.Suppliers.AnyAsync(s => s.SupplierID == dto.SupplierID))
            return BadRequest("Supplier not found");

        using var tr = await _context.Database.BeginTransactionAsync();

        var code = string.IsNullOrWhiteSpace(dto.Code)
            ? $"P-{Guid.NewGuid():N}".Substring(0, 12).ToUpper()
            : dto.Code;

        var product = new Product
        {
            Name = dto.Name,
            Price = dto.Price,
            Code = code,
            WarehouseID = dto.WarehouseID
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        _context.Transactions.Add(new Transaction
        {
            ProductID = product.ProductID,
            SupplierID = dto.SupplierID,
            Quantity = dto.InitialQuantity,
            TransactionType = "приход"
        });

        await _context.SaveChangesAsync();
        await tr.CommitAsync();

        return Ok();
    }
}

[ApiController]
[Route("api/warehouses")]
public class WarehousesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public WarehousesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetWarehouses()
    {
        return Ok(await _context.Warehouses.ToListAsync());
    }
}

[ApiController]
[Route("api/suppliers")]
public class SuppliersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SuppliersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetSuppliers()
    {
        return Ok(await _context.Suppliers.ToListAsync());
    }
}
