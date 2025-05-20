using System.Diagnostics.Metrics;
using Domain.ApiResponse;
using Domain.Entities;
using Infrastructure.Data;

namespace Infrastructure.Interfaces;

public interface IBooksService
{
    Task<Response<string>> CreateBookAsync(Books books);
    Task<Response<List<Books>>> GetBooksAsync();
    Task<Response<string>> UpdateBookAsync(Books books);
    Task<Response<string>> DeleteBookAsync(int Id);
    Task<Response<Books>> GetBookByIdAsync(int Id);
    Task<Response<List<Books>>> GetThePopularBook();
    Task<Response<string>> GetMostPopularGenre();
    Task<Response<List<Books>>> GetBooksAndBorrowingsCount();
}