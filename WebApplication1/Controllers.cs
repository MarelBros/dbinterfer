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
            .ToListAsync();

        return Ok(products);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] ProductCreate dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        bool supplierExists = await _context.Suppliers.AnyAsync(s => s.SupplierID == dto.SupplierID);
        bool warehouseExists = await _context.Warehouses.AnyAsync(w => w.WarehouseID == dto.WarehouseID);

        if (!supplierExists || !warehouseExists)
            return BadRequest("Supplier or Warehouse not found");

        var product = new Product
        {
            Name = dto.Name,
            WarehouseID = dto.WarehouseID,
            SupplierID = dto.SupplierID,
            Quantity = dto.Quantity,
            ProductCode = dto.ProductCode
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAllProducts), new { id = product.ProductID }, product);
    }

}
