using CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Interfaces
{
    /// <summary>
    /// Repository interface for managing Product entities.
    /// </summary>
    public interface IProductRepository
    {
        /// <summary>
        /// Retrieves a list of products filtered by optional brand and type parameters, and optionally sorted by a specified field.
        /// </summary>
        /// <param name="brand">The brand to filter products by (optional).</param>
        /// <param name="type">The type to filter products by (optional).</param>
        /// <param name="sort">The field to sort the products by (e.g., "priceAsc", "priceDesc", "name").</param>
        /// <returns>A list of products matching the specified filters and sort criteria.</returns>
        Task<IReadOnlyList<Product>> GetProductsAsync(string? brand, string? type, string? sort);


        /// <summary>
        /// Retrieves a list of all unique product brands from the database.
        /// </summary>
        /// <returns>A list of brand names.</returns>
        Task<IReadOnlyList<string>> GetBrandsAsync();

        /// <summary>
        /// Retrieves a list of all unique product types from the database.
        /// </summary>
        /// <returns>A list of product type names.</returns>
        Task<IReadOnlyList<string>> GetTypesAsync();


        /// <summary>
        /// Gets a product by its ID.
        /// </summary>
        Task<Product?> GetProductByIdAsync(int id);

        /// <summary>
        /// Adds a product to the context.
        /// </summary>
        void AddProduct(Product product);

        /// <summary>
        /// Marks a product as modified in the context.
        /// </summary>
        void UpdateProduct(Product product);

        /// <summary>
        /// Remove product from the context.
        /// </summary>
        void DeleteProduct(Product product);

        /// <summary>
        /// Checks if a product with the given ID exists.
        /// </summary>
        Task<bool> ProductExist(int id); 

        /// <summary>
        /// Saves all changes made in the context to the database.
        /// </summary>
        Task<bool> SaveChangesAsync();
    }

}
