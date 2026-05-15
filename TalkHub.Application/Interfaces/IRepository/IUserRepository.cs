using TalkHub.Domain.Entities;

namespace TalkHub.Application.Interfaces.IRepository;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<IEnumerable<User>> GetAllAsync();
    Task<(IEnumerable<User> Items, int TotalCount)> GetPagedAsync(string? searchTerm, bool? isActive, int pageNumber, int pageSize);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(User user);
    Task<bool> ExistsAsync(string username);
    Task<User?> GetByUsernameAsync(string username);
    Task<bool> ExistsByEmailAsync(string email, Guid? excludeId = null);
    Task<bool> ExistsByPhoneAsync(string phoneNumber, Guid? excludeId = null);
}
