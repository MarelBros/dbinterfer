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

    //получение продуктов
    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        var products = await _context.Products
            .Include(p => p.Supplier)
            .Include(p => p.Warehouse)
            .ToListAsync();

        return Ok(products);
    }

    //пост
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] Product product)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        bool supplierExists = await _context.Suppliers.AnyAsync(s => s.SupplierID == product.SupplierID);
        bool warehouseExists = await _context.Warehouses.AnyAsync(w => w.WarehouseID == product.WarehouseID);

        if (!supplierExists || !warehouseExists)
            return BadRequest("Supplier or Warehouse not found");
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAllProducts), new { id = product.ProductID }, product);
    }
}
