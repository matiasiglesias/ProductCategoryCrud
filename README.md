
# ProductCategoryCrud API

Esta es una API REST creada en .NET Core que permite gestionar productos y categorías en una base de datos SQLite. La 
tabla `Products` incluye un campo adicional llamado `Score`.

## Requisitos Previos

- .NET SDK 6.0 o superior
- SQLite
- Entity Framework Core

## Configuración del Proyecto

### 1. Crear el Proyecto

1. Abre una terminal y navega al directorio donde deseas crear el proyecto.
2. Ejecuta el siguiente comando para crear una aplicación Web API:

   ```bash
   dotnet new webapi -n ProductCategoryCrud
   ```

3. Navega al directorio del proyecto:

   ```bash
   cd ProductCategoryCrud
   ```

4. Instala las dependencias de Entity Framework Core y SQLite:

   ```bash
   dotnet add package Microsoft.EntityFrameworkCore
   dotnet add package Microsoft.EntityFrameworkCore.Sqlite
   dotnet add package Microsoft.EntityFrameworkCore.Tools
   ```

### 2. Configurar `AppDbContext`

1. Crea una carpeta llamada `Data` y dentro de ella un archivo `AppDbContext.cs` con el siguiente contenido:

   ```csharp
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
   ```

2. En `appsettings.json`, agrega la cadena de conexión para SQLite:

   ```json
   "ConnectionStrings": {
       "DefaultConnection": "Data Source=products.db"
   }
   ```

3. En `Program.cs`, configura `AppDbContext` para que use SQLite:

   ```csharp
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
   ```

### 3. Crear los Modelos `Category` y `Product`

1. Crea una carpeta `Models` y dentro de ella un archivo `Category.cs`:

   ```csharp
   namespace ProductCategoryCrud.Models
   {
       public class Category
       {
           public int Id { get; set; }
           public string Name { get; set; }
       }
   }
   ```

2. En la misma carpeta, crea el archivo `Product.cs` y agrega el campo `Score`:

   ```csharp
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
   ```

### 4. Crear las Migraciones y Actualizar la Base de Datos

1. Crea una migración inicial para la base de datos:

   ```bash
   dotnet ef migrations add InitialCreate
   ```

2. Aplica la migración a la base de datos:

   ```bash
   dotnet ef database update
   ```

### 5. Crear los Controladores `CategoriesController` y `ProductsController`

1. En la carpeta `Controllers`, crea el archivo `CategoriesController.cs`:

   ```csharp
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
   ```

2. Crea el archivo `ProductsController.cs`:

   ```csharp
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
   ```

### 6. Probar la API

1. Ejecuta el proyecto:

   ```bash
   dotnet run
   ```

2. Accede a [http://localhost:5287/swagger](http://localhost:5287/swagger) para probar los endpoints de `Categories` y `Products` en Swagger.

3. Realiza operaciones de CRUD y asegúrate de que el campo `Score` esté funcionando correctamente en la tabla `Products`.

---

Este README cubre desde la creación del proyecto hasta la adición del campo `Score` y las pruebas de los endpoints.
