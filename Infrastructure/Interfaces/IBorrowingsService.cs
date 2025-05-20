using Domain.Entities;
using Domain.ApiResponse;
using Domain.DTO;
using Domain.ApiResponse;

namespace Infrastructure.Interfaces;

public interface IBorrowingsService
{
    Task<Response<string>> CreateBorrowingAsync(Borrowings borrowings);
    Task<Response<string>> ReturnBookAsync(int BorrowingId);
    Task<Response<List<Borrowings>>> GetAllBorrowings();
    Task<Response<List<Borrowings>>> GetBorrowingsById(int memberId);
    Task<Response<int>> GetCountOfBorrowings();
    Task<Response<int>> GetAvgSumForLate();
    Task<Response<List<Borrowings>>> GetNotReturnedBooks();
    Task<Response<List<Borrowings>>> GetBooksWithoutCopies();
    Task<Response<string>> GetBooksWithoutGeting();
    Task<Response<int>> GetCountOfBorrowingsWithMember();
    Task<Response<ReaderDto>> GetFirstReaderWithOverdueAsync();
    Task<Response<List<Borrowings>>> GetTopFiveMembers();
    Task<Response<int>> GetAllWtraff();
    Task<Response<int>> GetBooksWithRasrochka();
    Task<Response<List<Borrowings>>> GetMembersWhoPay();
}