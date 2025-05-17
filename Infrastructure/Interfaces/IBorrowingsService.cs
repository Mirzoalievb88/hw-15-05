using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IBorrowingsService
{
    Task<string> CreateBorrowingAsync(Borrowings borrowings);
    Task<string> ReturnBookAsync(int BorrowingId);
    Task<List<Borrowings>> GetAllBorrowings();
    Task<List<Borrowings>> GetBorrowingsById(int memberId);
}