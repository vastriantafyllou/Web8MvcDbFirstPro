using System.Linq.Expressions;
using SchoolApp.Data;
using SchoolApp.Models;

namespace SchoolApp.Repositories;

public interface IUserRepository
{
    Task<User?> GetUserAsync(string username, string password);
    Task<User?> GetUserByUsernameAsync(string username);
    
    Task<PaginatedResult<User>> GetUsersAsync(int pageNumber, int pageSize,
        List<Expression<Func<User, bool>>> predicates);
}