using AppCore.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<UserDto?> AuthenticateAsync(LoginDto loginDto, CancellationToken cancellationToken = default);
        Task<bool> RegisterAsync(RegisterDto registerDto);
        Task<bool> IsUserExistsAsync(string username);
        Task<UserDto?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
        Task<UserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<UserDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<bool> InitTestUser();
    }
}
