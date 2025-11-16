using GadgetHubAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GadgetHubAPI.Data
{
    public class ProductRepo
    {
        private readonly AppDbContext _context;

        public ProductRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Product?> GetByGlobalIdAsync(string globalId)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.GlobalId == globalId);
        }

        public async Task<Product?> GetByIdAsync(int productId)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == productId);
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product> AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> ExistsAsync(string globalId)
        {
            return await _context.Products
                .AnyAsync(p => p.GlobalId == globalId);
        }

        public async Task<bool> DeleteAsync(string globalId)
        {
            var product = await GetByGlobalIdAsync(globalId);
            if (product == null) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
