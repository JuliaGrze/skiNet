using CORE.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly StoreContext context;

        public ProductsController(StoreContext context)
        {
            this.context = context;
        }


        /// <summary>
        /// Retruns all products from database
        /// </summary>
        /// <returns>A list of all available products</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await context.Products.ToListAsync();
        }

        /// <summary>
        /// Retrieves a single product by its ID
        /// </summary>
        /// <param name="id">The ID of the product to retrieve</param>
        /// <returns>The product with the specified ID if found; otherwise, a 404 Not Found response</returns>

        [HttpGet("{id:int}")] //api/products/2
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await context.Products.FindAsync(id);

            if (product == null)
                return NotFound(); //status - 404 Not Found

            return product;
        }

        /// <summary>
        /// Creates a new product and saves it to the database
        /// </summary>
        /// <param name="product">The product to create</param>
        /// <returns>The crated product</returns>

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            context.Products.Add(product); //no need AddAsync because AddAsync doesn't really do I/O - it just prepares the entity
            await context.SaveChangesAsync();

            return product;
        }

        /// <summary>
        /// Updates an existing product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to update.</param>
        /// <param name="product">The updated product data.</param>
        /// <returns>204 No Content if successful; 400 or 404 if invalid.</returns>

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateProduct(int id, Product product)
        {
            if(product.Id != id)
                return BadRequest("Product ID mismatch");

            if (!await ProductExistsAsync(id))
                return NotFound("Product not found");

            //This product object has been modified and should be updated in the database on the next SaveChanges()
            context.Entry(product).State = EntityState.Modified;
            await context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Deletes an exisiting product by its ID
        /// </summary>
        /// <param name="id">The ID of the product to delete</param>
        /// <returns>204 No Content if successful, 404 Not found if product wasnt find</returns>
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await context.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            context.Products.Remove(product);

            await context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> ProductExistsAsync(int id)
        {
            return await context.Products.AnyAsync(p => p.Id == id);
        }

    }
}
