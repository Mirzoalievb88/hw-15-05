using Domain.Entities;
using Infrastructure.Interfaces;
using Npgsql;
using Dapper;
using Infrastructure.Data;
using Domain.ApiResponse;
using System.Net;

namespace Infrastructure.Services;

public class BooksService(DataContext context) : IBooksService
{
    public async Task<Response<string>> CreateBookAsync(Books books)
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"insert into Books(Title, Genre, PublicationYear, TotalCopies, AvailableCopies)
                                values (@Title, @Genre, @PublicationYear, @TotalCopies, @AvailableCopies)";
            var result = await connection.ExecuteAsync(cmd, books);
            if (result == null)
            {
                return new Response<string>("Result is null", HttpStatusCode.NotFound);
            }
            return new Response<string>(default, "All worked");
        }
    }
    public async Task<Response<List<Books>>> GetBooksAsync()
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"select * from Books";
            var result = await connection.QueryAsync<Books>(cmd);
            if (result == null)
            {
                return new Response<List<Books>>("Result is null", HttpStatusCode.NotFound);
            }
            return new Response<List<Books>>(result.ToList(), "All worked");
        }
    }
 
    public async Task<Response<string>> UpdateBookAsync(Books books)
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"update Books
                                set Title = @Title, Genre = @Genre, PublicationYear = @PublicationYear, AvailableCopies = @AvailableCopies";
            var result = await connection.ExecuteAsync(cmd, books);
            if (result == null)
            {
                return new Response<string>("Result is null", HttpStatusCode.NotFound);
            }
            return new Response<string>(result.ToString(), "All worked");
        }
    }
    public async Task<Response<string>> DeleteBookAsync(int Id)
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"delete from Books
                                where Id = @Id";
            var result = await connection.ExecuteAsync(cmd, @Id = Id);
            if (result == null)
            {
                return new Response<string>("Result is null", HttpStatusCode.NotFound);
            }
            return new Response<string>(result.ToString(), "All worked");
        }
    }


    public async Task<Response<Books>> GetBookByIdAsync(int id)
    {
        using var connection = await context.GetConnectionAsync();
        var cmd = @"select * from books
                    where bookid = @bookid";
        var result = await connection.QuerySingleOrDefaultAsync<Books>(cmd, new { bookid = id });
        if (result == null)
        {
            return new Response<Books>("Result is null", HttpStatusCode.NotFound);
        }
        return new Response<Books>(result, "All worked");
    }

    public async Task<Response<List<Books>>> GetThePopularBook()
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"select count(b.*), bk.* from books bk
                                join borrowings b on b.BookId = bk.BookId
                                group by bk.bookId";
            var result = await connection.QueryAsync<Books>(cmd);
            if (result == null)
            {
                return new Response<List<Books>>("Result is null", HttpStatusCode.NotFound);
            }
            return new Response<List<Books>>(result.ToList(), "All worked");
        }
    }

    public async Task<Response<string>> GetMostPopularGenre()
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"Select b.Genre
                                    from Borrowings br
                                    join Books b ON br.BookId = b.Id
                                    Group By b.Genre
                                    Order By Count(*) Desc
                                    limit 1;
                                ";
            var result = await connection.QueryFirstOrDefaultAsync<string>(cmd);
            if (result == null)
            {
                return new Response<string>("Result is null", HttpStatusCode.NotFound);
            }
            return new Response<string>(result, "All worked");
        }
    }

    public async Task<Response<List<Books>>> GetBooksAndBorrowingsCount()
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"select b.Id, b.Title, b.Author
                                    from Books b
                                    join Borrowings br ON b.Id = br.BookId
                                    group BY b.Id, b.Title, b.Author
                                    having count(*) > 5";
            var result = await connection.QueryAsync<Books>(cmd);
            if (result == null)
            {
                return new Response<List<Books>>("Result is null", HttpStatusCode.NotFound);
            }
            return new Response<List<Books>>(result.ToList(), "All worked");
        }
    }
}