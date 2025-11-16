using GadgetHubAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GadgetHubAPI.Data
{
    public class UserRepo
    {
        private readonly AppDbContext _db;
        public UserRepo(AppDbContext db) { _db = db; }

        public User? GetByUsername(string username) =>
            _db.Users.AsNoTracking().FirstOrDefault(u => u.Username == username);

        public async Task<User?> GetByUsernameAsync(string username) =>
            await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == username);

        public bool Exists(string username) =>
            _db.Users.Any(u => u.Username == username);

        public async Task<bool> ExistsAsync(string username) =>
            await _db.Users.AnyAsync(u => u.Username == username);

        public void Add(User user)
        {
            _db.Users.Add(user);
            _db.SaveChanges();
        }

        public async Task AddAsync(User user)
        {
            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
