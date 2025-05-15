using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IBorrowingsService
{
    Task<int> CreateBorrowingsAsync(Borrowings borrowings);
    // Task<List<Borrowings>> GetBorrowingsAsync();
    Task<List<Borrowings>> GetBorrowingMemberByIdAsync(int Id);
    // Task<List<Borrowings>> GetBorrowingsWithFiltre();
}