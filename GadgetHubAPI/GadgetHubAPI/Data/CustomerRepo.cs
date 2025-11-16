using GadgetHubAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GadgetHubAPI.Data
{
    public class CustomerRepo
    {
        private readonly AppDbContext _db;

        public CustomerRepo(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _db.Customers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Customer?> GetByUserIdAsync(int userId)
        {
            return await _db.Customers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            return await _db.Customers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<List<Customer>> GetAllAsync()
        {
            return await _db.Customers
                .Include(c => c.User)
                .ToListAsync();
        }

        public async Task AddAsync(Customer customer)
        {
            await _db.Customers.AddAsync(customer);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Customer customer)
        {
            _db.Customers.Update(customer);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var customer = await _db.Customers.FindAsync(id);
            if (customer != null)
            {
                _db.Customers.Remove(customer);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _db.Customers.AnyAsync(c => c.Email == email);
        }

        public async Task<bool> ExistsByPhoneAsync(string phoneNumber)
        {
            return await _db.Customers.AnyAsync(c => c.PhoneNumber == phoneNumber);
        }
    }
}
