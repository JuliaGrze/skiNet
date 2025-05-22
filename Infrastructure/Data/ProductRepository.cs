using CORE.Entities;
using CORE.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class ProductRepository : IProductRepository
    {
        private readonly StoreContext _context;

        public ProductRepository(StoreContext context)
        {
            _context = context;
        }

        public void AddProduct(Product product)
        {
            _context.Products.Add(product);
        }

        public void DeleteProduct(Product product)
        {
            _context.Products.Remove(product);
        }

        public async Task<IReadOnlyList<string>> GetBrandsAsync()
        {
            return await _context.Products.Select(x => x.Brand) //Why do we use Select and not Where in GetBrandsAsync() ? Because you want to get a list of brands of all products, not filter only some.
                .Distinct() //only unique values
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<IReadOnlyList<Product>> GetProductsAsync(string? brand, string? type, string? sort)
        {
            var query = _context.Products.AsQueryable(); // Allows dynamic query building

            // filter by brand
            if (!string.IsNullOrWhiteSpace(brand))
            {
                query = query.Where(p => p.Brand.ToLower() == brand.ToLower());
            }

            // filter by type
            if (!string.IsNullOrWhiteSpace(type))
            {
                query = query.Where(p => p.Type.ToLower() == type.ToLower());
            }

            // Sort the products based on the 'sort' parameter.
            // Supports: "priceAsc", "priceDesc". Defaults to sorting by Name (ascending).
            query = sort switch
            {
                "priceAsc" => query.OrderBy(p => p.Price),
                "priceDesc" => query.OrderByDescending(p => p.Price),
                _ => query.OrderBy(p => p.Name) // default sort
            };
            

            return await query.ToListAsync(); //Executes a query and returns data from the database
        }

        public async Task<IReadOnlyList<string>> GetTypesAsync()
        {
            return await _context.Products.Select(x => x.Type)
                .Distinct()
                .ToListAsync();
        }

        public async Task<bool> ProductExist(int id)
        {
            return await _context.Products.AnyAsync(p => p.Id == id);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void UpdateProduct(Product product)
        {
            _context.Entry(product).State = EntityState.Modified;
        }
    }
}
