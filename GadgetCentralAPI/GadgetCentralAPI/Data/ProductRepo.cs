using GadgetCentralAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GadgetCentralAPI.Data
{
    public class ProductRepo
    {
        private readonly AppDbContext _context;
        public ProductRepo(AppDbContext context) => _context = context;

        public IQueryable<Product> GetProducts() => _context.Products.AsQueryable();
        
        public Product? GetProductById(int id)
        {
            return _context.Products.FirstOrDefault(p => p.ProductId == id);
        }

        public Product? GetProductByGlobalId(string globalId)
        {
            return _context.Products.FirstOrDefault(p => p.GlobalId == globalId);
        }

        // Async methods needed by OrderController
        public async Task<Product?> GetByGlobalIdAsync(string globalId)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.GlobalId == globalId);
        }

        public async Task<bool> UpdateAsync(Product product)
        {
            try
            {
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Add(Product product) => _context.Products.Add(product);
        public void Remove(Product product) => _context.Products.Remove(product);
        public bool Save() => _context.SaveChanges() > 0;
    }
}