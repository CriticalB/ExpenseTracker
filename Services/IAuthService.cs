using ExpenseTracker.DTOs;

namespace ExpenseTracker.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequest);
}
