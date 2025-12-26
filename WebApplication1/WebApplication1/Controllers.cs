using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


//////////////////////////////ТРАНЗАКЦИИ
[ApiController]
[Route("api/transactions")]
public class TransactionsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TransactionsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET api/transactions
    [HttpGet]
    public async Task<IActionResult> GetTransactions()
    {
        var list = await _context.Transactions
            .Include(t => t.Product)
            .Include(t => t.Supplier)
            .OrderByDescending(t => t.TransactionDate)
            .Select(t => new
            {
                t.TransactionID,
                ProductName = t.Product.Name,
                SupplierName = t.Supplier != null ? t.Supplier.Name : null,
                t.Quantity,
                t.TransactionType,
                t.TransactionDate
            })
            .ToListAsync();

        return Ok(list);
    }

    // POST api/transactions
    [HttpPost]
    public async Task<IActionResult> CreateTransaction([FromBody] TransactionDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!await _context.Products.AnyAsync(p => p.ProductID == dto.ProductID))
            return BadRequest("Product not found");

        if (dto.SupplierID != null &&
            !await _context.Suppliers.AnyAsync(s => s.SupplierID == dto.SupplierID))
            return BadRequest("Supplier not found");

        var tr = new Transaction
        {
            ProductID = dto.ProductID,
            SupplierID = dto.SupplierID,
            Quantity = dto.Quantity,
            TransactionType = dto.TransactionType,
            TransactionDate = DateTime.Now
        };

        _context.Transactions.Add(tr);
        await _context.SaveChangesAsync();

        return StatusCode(201);
    }

    // PUT api/transactions/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTransaction(int id, [FromBody] TransactionDto dto)
    {
        var tr = await _context.Transactions.FindAsync(id);
        if (tr == null)
            return NotFound();

        tr.ProductID = dto.ProductID;
        tr.SupplierID = dto.SupplierID;
        tr.Quantity = dto.Quantity;
        tr.TransactionType = dto.TransactionType;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE api/transactions/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTransaction(int id)
    {
        var tr = await _context.Transactions.FindAsync(id);
        if (tr == null)
            return NotFound();

        _context.Transactions.Remove(tr);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

//////////////////////////////ПРОДУКТЫ

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
    .Include(p => p.Warehouse)
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products
            .Include(p => p.Transactions)
            .FirstOrDefaultAsync(p => p.ProductID == id);

        if (product == null)
            return NotFound();

        _context.Transactions.RemoveRange(product.Transactions);
        _context.Products.Remove(product);

        await _context.SaveChangesAsync();
        return NoContent();
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductCreateDto dto)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return NotFound();

        product.Name = dto.Name;
        product.Price = dto.Price;
        product.Code = dto.Code ?? product.Code;

        await _context.SaveChangesAsync();
        return NoContent();
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
//////////////////////////////СКЛАДЫ
[ApiController]
[Route("api/warehouses")]
public class WarehousesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public WarehousesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET api/warehouses
    [HttpGet]
    public async Task<IActionResult> GetWarehouses()
    {
        return Ok(await _context.Warehouses.ToListAsync());
    }

    // POST api/warehouses
    [HttpPost]
    public async Task<IActionResult> CreateWarehouse([FromBody] WarehouseDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var warehouse = new Warehouse
        {
            Name = dto.Name
        };

        _context.Warehouses.Add(warehouse);
        await _context.SaveChangesAsync();

        return StatusCode(201);
    }

    // PUT api/warehouses/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateWarehouse(int id, [FromBody] WarehouseDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var warehouse = await _context.Warehouses.FindAsync(id);
        if (warehouse == null)
            return NotFound();

        warehouse.Name = dto.Name;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE api/warehouses/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWarehouse(int id)
    {
        // ❗ проверяем продукты, а не транзакции
        var hasProducts = await _context.Products
            .AnyAsync(p => p.WarehouseID == id);

        if (hasProducts)
            return BadRequest("Warehouse contains products");

        var warehouse = await _context.Warehouses.FindAsync(id);
        if (warehouse == null)
            return NotFound();

        _context.Warehouses.Remove(warehouse);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

//////////////////////////////ПОСТАВЩИКИ
[ApiController]
[Route("api/suppliers")]
public class SuppliersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SuppliersController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/suppliers
    [HttpGet]
    public async Task<IActionResult> GetSuppliers()
    {
        return Ok(await _context.Suppliers.ToListAsync());
    }

    // POST: api/suppliers
    [HttpPost]
    public async Task<IActionResult> CreateSupplier([FromBody] SupplierDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var supplier = new Supplier
        {
            Name = dto.Name,
            INN = dto.Inn,
            Phone = dto.Phone
        };

        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync();

        // Flutter ждёт 201
        return StatusCode(201);
    }

    // PUT: api/suppliers/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSupplier(int id, [FromBody] SupplierDto dto)
    {
        var supplier = await _context.Suppliers.FindAsync(id);
        if (supplier == null)
            return NotFound();

        supplier.Name = dto.Name;
        supplier.INN = dto.Inn;
        supplier.Phone = dto.Phone;

        await _context.SaveChangesAsync();

        // Flutter ждёт 204
        return NoContent();
    }

    // DELETE: api/suppliers/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSupplier(int id)
    {
        var supplier = await _context.Suppliers.FindAsync(id);
        if (supplier == null)
            return NotFound();

        _context.Suppliers.Remove(supplier);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

[ApiController]
[Route("api/login")]
public class LoginController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public LoginController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest("Имя и пароль обязательны");

        // Поиск пользователя в БД
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Name == dto.Name && u.Password == dto.Password);

        if (user == null)
            return Unauthorized("Неверный логин или пароль");

        // Возвращаем только Name и Tier
        var response = new LoginResponseDto
        {
            Name = user.Name,
            Tier = user.Tier
        };

        return Ok(response);
    }
}
