Proyecto ejemplo de API
Paso 1: Crear el Proyecto
1.	Abre una terminal y navega al directorio donde deseas crear el proyecto.
2.	Ejecuta el siguiente comando para crear una aplicación Web API en .NET Core:
dotnet new webapi -n ProductCategoryCrud
3.	Navega al directorio del proyecto:
cd ProductCategoryCrud
4.	Abre el archivo ProductCategoryCrud.csproj y asegúrate de que tenga las dependencias básicas necesarias. Luego instala Entity Framework Core y SQLite:
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Tools
________________________________________
Paso 2: Configurar la Base de Datos con AppDbContext
1.	En el proyecto, crea una carpeta llamada Data.
2.	Dentro de la carpeta Data, crea un archivo llamado AppDbContext.cs con el siguiente contenido:
using Microsoft.EntityFrameworkCore;
using ProductCategoryCrud.Models;

namespace ProductCategoryCrud.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}
3.	En el archivo appsettings.json, agrega la cadena de conexión para SQLite:
"ConnectionStrings": {
    "DefaultConnection": "Data Source=products.db"
}
4.	En Program.cs, configura AppDbContext para que use SQLite:
using Microsoft.EntityFrameworkCore;
using ProductCategoryCrud.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar AppDbContext con SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
________________________________________
Paso 3: Crear los Modelos Category y Product
1.	Crea una carpeta llamada Models en el proyecto.
2.	Dentro de la carpeta Models, crea un archivo Category.cs:
namespace ProductCategoryCrud.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
3.	En la misma carpeta, crea el archivo Product.cs y agrega el campo Score:
namespace ProductCategoryCrud.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public int Score { get; set; }  // Campo Score
    }
}
________________________________________
Paso 4: Crear las Migraciones y Actualizar la Base de Datos
1.	Ejecuta el siguiente comando para crear una migración inicial:
dotnet ef migrations add InitialCreate
2.	Aplica la migración para crear las tablas en la base de datos:
dotnet ef database update
________________________________________
Paso 5: Crear los Controladores CategoriesController y ProductsController
1.	En la carpeta Controllers, crea el archivo CategoriesController.cs:
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductCategoryCrud.Data;
using ProductCategoryCrud.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductCategoryCrud.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // CRUD actions for Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();
            return category;
        }

        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, Category category)
        {
            if (id != category.Id) return BadRequest();
            _context.Entry(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
2.	Luego, crea el archivo ProductsController.cs:
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductCategoryCrud.Data;
using ProductCategoryCrud.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductCategoryCrud.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // CRUD actions for Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            return product;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id) return BadRequest();
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
________________________________________
Paso 6: Probar la API
1.	Ejecuta el proyecto:
dotnet run
2.	Abre http://localhost:5287/swagger para probar los endpoints de Categories y Products en Swagger.
3.	Prueba las operaciones de CRUD para asegurarte de que todo funciona correctamente, incluyendo el campo Score en Products.

