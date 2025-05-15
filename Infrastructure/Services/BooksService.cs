using Domain.Entities;
using Infrastructure.Interfaces;
using Npgsql;
using Dapper;
using Infrastructure.Data;

namespace Infrastructure.Services;

public class BooksService : IBooksService
{
    private readonly DataContext context = new DataContext();

    public async Task<int> CreateBookAsync(Books books)
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"insert into Books(Title, Genre, PublicationYear, TotalCopies, AvailableCopies)
                                values (@Title, @Genre, @PublicationYear, @TotalCopies, @AvailableCopies)";
            var result = await connection.ExecuteAsync(cmd, books);
            return result;
        }
    }
    public async Task<List<Books>> GetBooksAsync()
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"select * from Books";
            var result = await connection.QueryAsync<Books>(cmd);
            return result.ToList();
        }
    }
 
    public async Task<int> UpdateBookAsync(Books books)
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"update Books
                                set Title = @Title, Genre = @Genre, PublicationYear = @PublicationYear, AvailableCopies = @AvailableCopies";
            var result = await connection.ExecuteAsync(cmd, books);
            return result;
        }
    }
    public async Task<int> DeleteBookAsync(int Id)
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"delete from Books
                                where Id = @Id";
            var result = await connection.ExecuteAsync(cmd, @Id = Id);
            return result;
        }
    }


    // public async Task<Books?> GetBookByIdAsync(int id)
    // {
    //     using var connection = await context.GetConnectionAsync();
    //     var cmd = @"select * from books
    //                 where bookid = @bookid";
    //     var result = await connection.QuerySingleOrDefaultAsync<Books>(cmd, new { bookid = id });
    //     return result;
    // }
}