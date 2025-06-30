using API.RequestHelpers;
using CORE.Entities;
using CORE.Interfaces;
using CORE.Specifications;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        /// <summary>
        /// Retrieves all products from the database, optionally filtered by brand and type, and optionally sorted.
        /// </summary>
        /// <param name="brand">The brand to filter products by (optional).</param>
        /// <param name="type">The type to filter products by (optional).</param>
        /// <param name="sort">The sort order for the products. Supported values: "priceAsc", "priceDesc", "name".</param>
        /// <returns>A list of products matching the optional brand and type filters, sorted as specified.</returns>
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts([FromQuery]ProductSpecificationParams productSpecification)
        {
            var specification = new ProductSpecification(productSpecification);

            return await CreatePageResult<Product>(_unitOfWork.Repository<Product>(), specification, productSpecification.PageIndex, productSpecification.PageSize);
        }

        /// <summary>
        /// Retrieves a single product by its ID
        /// </summary>
        /// <param name="id">The ID of the product to retrieve</param>
        /// <returns>The product with the specified ID if found; otherwise, a 404 Not Found response</returns>

        [HttpGet("{id:int}")] //api/products/2
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);

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
            _unitOfWork.Repository<Product>().Add(product); //no need Async method because AddAsync doesn't really do I/O - it just prepares the entity

            if (await _unitOfWork.Complete())
                return CreatedAtAction("GetProduct", new { id = product.Id }, product); //HTTP 201 Created
            //CreatedAtAction(actionName, routeValues, value)
            //ActionName: nazwa metody (akcji), która może pobrać nowo utworzony zasób — w tym przypadku "GetProduct" - metoda wyzej
            //routeValues: np. { id = product.Id } — wartosci do podstawienia w URL tej akcji
            //value: obiekt, który zostanie zwrocony w odpowiedzi (product)

            return BadRequest("Problem creating product");
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
            _unitOfWork.Repository<Product>().Update(product);

            if(await _unitOfWork.Complete())
                return NoContent();

            return BadRequest("Problem updating product");
        }

        /// <summary>
        /// Deletes an exisiting product by its ID
        /// </summary>
        /// <param name="id">The ID of the product to delete</param>
        /// <returns>204 No Content if successful, 404 Not found if product wasnt find</returns>
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);

            if (product == null)
                return NotFound();

            _unitOfWork.Repository<Product>().Remove(product);

            if (await _unitOfWork.Complete())
                return NoContent();

            return StatusCode(500, "Problem deleting the product");
        }

        /// <summary>
        /// Retrives a list of unique product brands
        /// </summary>
        /// <returns> A list of product brand name </returns>
        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
        {
            var specification = new BrandListSpecifications();
            return Ok( await _unitOfWork.Repository<Product>().ListAsyncWithSpec<string>(specification));
        }


        /// <summary>
        /// Retrives a list of unique product types
        /// </summary>
        /// <returns> A list of product type name </returns>
        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
        {
            var specifications = new TypeListSpecification();
            return Ok(await _unitOfWork.Repository<Product>().ListAsyncWithSpec<string>(specifications));
        }

        private async Task<bool> ProductExistsAsync(int id)
        {
            return await _unitOfWork.Repository<Product>().Exist(id);
        }

    }
}
