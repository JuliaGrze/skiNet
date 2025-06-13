using CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Interfaces
{
    /// <summary>
    /// Defines the contract for shopping cart operations. like a setting, getting and delete shopping cart
    /// </summary>
    public interface ICartService
    {
        /// <summary>
        /// Retrieves the shopping cart for the specified key.
        /// </summary>
        /// <param name="key">The unique identifier for the shopping cart.</param>
        /// <returns>The corresponding ShoppingCart, or null if not found.</returns>
        Task<ShoppingCart?> GetCartAsync(string key);

        /// <summary>
        /// Saves or updates the shopping cart in the storage.
        /// </summary>
        /// <param name="cart">The ShoppingCart object to save or update.</param>
        /// <returns>The updated ShoppingCart, or null if the operation fails.</returns>
        Task<ShoppingCart?> SetCartAsync(ShoppingCart cart);

        /// <summary>
        /// Deletes the shopping cart for the specified key.
        /// </summary>
        /// <param name="key">The unique identifier for the shopping cart to delete.</param>
        /// <returns>True if deletion was successful; otherwise, false.</returns>
        Task<bool> DeleteCartAsync(string key);

    }
}
