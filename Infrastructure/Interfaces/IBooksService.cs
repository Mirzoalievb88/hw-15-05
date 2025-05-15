using System.Diagnostics.Metrics;
using Domain.Entities;
using Infrastructure.Data;

namespace Infrastructure.Interfaces;

public interface IBooksService
{
    Task<int> CreateBookAsync(Books books);
    Task<List<Books>> GetBooksAsync();
    Task<int> UpdateBookAsync(Books books);
    Task<int> DeleteBookAsync(int Id);
    // Task<Books> GetBookByIdAsync(int Id);
}