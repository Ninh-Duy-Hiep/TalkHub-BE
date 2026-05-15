using Microsoft.EntityFrameworkCore;
using TalkHub.Application.Interfaces.IRepository;
using TalkHub.Domain.Entities;

namespace TalkHub.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.AsNoTracking().ToListAsync();
    }

    public async Task<(IEnumerable<User> Items, int TotalCount)> GetPagedAsync(string? searchTerm, bool? isActive, int pageNumber, int pageSize)
    {
        var query = _context.Users.AsNoTracking().AsQueryable();

        query = query.Where(u => !u.IsDeleted);

        if (isActive.HasValue)
        {
            query = query.Where(u => u.IsActive == isActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearchTerm = searchTerm.ToLower();
            query = query.Where(u =>
                u.FullName.ToLower().Contains(lowerSearchTerm));
        }

        var totalCount = await query.CountAsync();

        var users = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (users, totalCount);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(User user)
    {
        user.IsDeleted = true;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(string username)
    {
        return await _context.Users.AnyAsync(u => u.Username == username && !u.IsDeleted);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<bool> ExistsByEmailAsync(string email, Guid? excludeId = null)
    {
        return await _context.Users.AnyAsync(x => x.Email == email && !x.IsDeleted && x.Id != excludeId);
    }

    public async Task<bool> ExistsByPhoneAsync(string phoneNumber, Guid? excludeId = null)
    {
        return await _context.Users.AnyAsync(x => x.PhoneNumber == phoneNumber && !x.IsDeleted && x.Id != excludeId);
    }
}
