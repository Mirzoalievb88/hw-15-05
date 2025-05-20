using Domain.Entities;
using Domain.DTO;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Npgsql;
using Dapper;
using System.Data.Common;
using Domain.ApiResponse;
using System.Net;

namespace Infrastructure.Services;

public class BorrowingsService(DataContext context) : IBorrowingsService
{
    public async Task<Response<List<Borrowings>>> GetAllBorrowingsAsync()
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            string cmd = @$"select * from Borrowings";
            var result = await connection.QueryAsync<Borrowings>(cmd);
            if (result == null)
            {
                return new Response<List<Borrowings>>("Result is null", HttpStatusCode.NotFound);
            }
            return new Response<List<Borrowings>>(result.ToList(), "All worked");
        }
    }
    public async Task<Response<Borrowings>> GetBorrowingByMemberId(int memberId)
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();

            string cmd = $@"Select * from Borrowings Where memberId = @memberId";
            var result = await connection.QueryFirstOrDefaultAsync<Borrowings>(cmd, new { memberId = memberId });
            if (result == null)
            {
                return new Response<Borrowings>("Result is null", HttpStatusCode.NotFound);
            }
            return new Response<Borrowings>(result, "All worked");
        }
    }

    public async Task<Response<string>> CreateBorrowingAsync(Borrowings borrowings)
    {
        using (var connection = await context.GetConnectionAsync())
        {
            var bookCommand = @$"select * from books where bookId = @Id";
            var book = await connection.QueryFirstOrDefaultAsync<Books>(bookCommand, new { Id = borrowings.BookId });
            if (bookCommand == null)
            {
                return new Response<string>("bookCommand is null", HttpStatusCode.NotFound);
            }
            if (book!.AvailableCopies <= 0)
            {
                return new Response<string>("Available copies are not available", HttpStatusCode.NotFound);
            }
            if (borrowings.BorrowDate >= borrowings.DueDate)
            {
                return new Response<string>("Borrowing due date is earlier", HttpStatusCode.NotFound);
            }
            var borrowingCommand = @"insert into borrowings (bookid, memberId, borrowDate, dueDate)
                                            values (@bookId, @memberId, @borrowDate, @dueDate)";
            var result = await connection.ExecuteAsync(borrowingCommand, borrowings);
            if (result == 0)
            {
                return new Response<string>("Borrowing not created", HttpStatusCode.NotFound);
            }

            var updateBookCommand = @"update books set availableCopies = availableCopies - 1 
                                                where BookId = @Id";
            await connection.ExecuteAsync(updateBookCommand, new { Id = borrowings.BookId });

            return new Response<string>(default, "Borrowing created");
        }
    }

    public async Task<Response<string>> ReturnBookAsync(int BorrowingId)
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var borrowingCommand = @"select * from borrowings where borrowingId = @Id";
            var borrowing = await connection.QueryFirstOrDefaultAsync<Borrowings>(borrowingCommand, new {Id = BorrowingId});
            if (borrowing == null)
            {
                return new Response<string>("Borrowing not found", HttpStatusCode.NotFound);
            }

            borrowing.ReturnDate = DateTime.Now;
            if (borrowing.ReturnDate > borrowing.DueDate)
            {
                var days = borrowing.ReturnDate.Value.Day - borrowing.DueDate.Day;
                borrowing.Fine = days * 10;
            }

            var updateBorrowingCommand = "update borrowings set returnDate = @returnDate, fine = @fine where borrowingId = @Id";
            var result = await connection.ExecuteAsync(updateBorrowingCommand, borrowing);
            if (result == 0)
            {
                return new Response<string>("Borrowing not updated", HttpStatusCode.NotFound);
            }

            var updateBookCommand = @"update books set availableCopies = availableCopies + 1 where bookId = @Id";
            await connection.ExecuteAsync(updateBookCommand, new { Id = borrowing.BookId });

            return new Response<string>(default, "Borrowing updated");
        }
    }

    public async Task<Response<List<Borrowings>>> GetAllBorrowings()
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"select * from borrowings";
            var result = await connection.QueryAsync<Borrowings>(cmd);
            if (result == null)
            {
                return new Response<List<Borrowings>>("Result is null", HttpStatusCode.NotFound);
            }
            return new Response<List<Borrowings>>(result.ToList(), "All worked");
        }
    }

    public async Task<Response<List<Borrowings>>> GetBorrowingsById(int memberId)
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"select b.*, m.* from borrowings
                                join Members m on m.memberId = b.memberId
                                where Id = @Id";
            var result = await connection.QueryAsync<Borrowings>(cmd, new { @Id = memberId });
            if (result == null)
            {
                return new Response<List<Borrowings>>("Result is null", HttpStatusCode.NotFound);
            }
            return new Response<List<Borrowings>>(result.ToList(), "All worked");
        }
    }

    public async Task<Response<int>> GetCountOfBorrowings()
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"select count(*) from borrowings
                                where returnDate < Duedate";
            var result = await connection.ExecuteScalarAsync<int>(cmd);
            if (result == null)
            {
                return new Response<int>("Result is null", HttpStatusCode.NotFound);
            }
            return new Response<int>(result, "All Worked");
        }
    }

    public async Task<Response<int>> GetAvgSumForLate()
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"SELECT AVG(EXTRACT(DAY FROM (ReturnDate - DueDate)) * 1.5)
                                    FROM Borrowings
                                    WHERE ReturnDate > DueDate;
                                    ";
            var result = await connection.ExecuteScalarAsync<int>(cmd);
            if (result == null)
            {
                return new Response<int>("Result is null", HttpStatusCode.InternalServerError);
            }
            return new Response<int>(result, "All worked");
        }
    }

    public async Task<Response<List<Borrowings>>> GetNotReturnedBooks()
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"SELECT b.BorrowingId, b.BookId, b.MemberId, b.BorrowDate, b.DueDate
                                    FROM Borrowings b
                                    WHERE b.ReturnDate IS NULL;
                                    ";
            var result = await connection.QueryAsync<Borrowings>(cmd);
            if (result == null)
            {
                return new Response<List<Borrowings>>("Result is null", HttpStatusCode.NotFound);
            }
            return new Response<List<Borrowings>>(result.ToList(), "All worked");
        }
    }

    public async Task<Response<List<Borrowings>>> GetBooksWithoutCopies()
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"select b.BookId, b.Title, b.Genre from Borrowings bo
                                join Books b on b.BokkId = bo.BookId
                                where Totalcopies < 2";
            var result = await connection.QueryAsync<Borrowings>(cmd);
            if (result == null)
            {
                return new Response<List<Borrowings>>("Result is null", HttpStatusCode.NotFound);
            }
            return new Response<List<Borrowings>>(result.ToList(), "All worked");
        }
    }

    public async Task<Response<string>> GetBooksWithoutGeting()
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"sSELECT COUNT(*) 
                                    FROM Books b
                                    WHERE NOT EXISTS (
                                        SELECT 1 FROM Borrowings br WHERE br.BookId = b.Id
                                    );
                                    ";
            var result = await connection.ExecuteScalarAsync<int>(cmd);
            if (result == null)
            {
                return new Response<string>("Result is null", HttpStatusCode.NotFound);
            }
            return new Response<string>(result.ToString(), "All worked");
        }
    }

    public async Task<Response<int>> GetCountOfBorrowingsWithMember()
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"select count(b.*), m.* from borrowings b
                                group by m.memberId";
            var result = await connection.ExecuteScalarAsync<int>(cmd);
            if (result == null)
            {
                return new Response<int>("Result is null", HttpStatusCode.NotFound);
            }
            return new Response<int>(result, "All worked");
        }
    }

    public async Task<Response<ReaderDto>> GetFirstReaderWithOverdueAsync()
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"SELECT r.Id, r.Name
                                    FROM Readers r
                                    JOIN Borrowings b ON r.Id = b.ReaderId
                                    WHERE b.ReturnDate > b.DueDate
                                    ORDER BY b.ReturnDate
                                    LIMIT 1;
                                ";
            var result = await connection.QueryFirstOrDefaultAsync<ReaderDto>(cmd);
            if (result == null)
            {
                return new Response<ReaderDto>("Result is null", HttpStatusCode.NotFound);
            }
            return new Response<ReaderDto>(result, "All worked");
        }
    }

    public async Task<Response<List<Borrowings>>> GetTopFiveMembers()
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"select m.* from Borrowings br
                                join Members m on m.memberId = br.memberId
                                limit 5";
            var result = await connection.QueryAsync<Borrowings>(cmd);
            if (result == null)
            {
                return new Response<List<Borrowings>>("Result is null", HttpStatusCode.NotFound);
            }
            return new Response<List<Borrowings>>(result.ToList(), "All worked");
        }
    }

    public async Task<Response<int>> GetAllWtraff()
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"select sum(extract(day from (ReturnDate - DueDate)) * 1.0)
                                    from Borrowings
                                    where ReturnDate > DueDate;
                                ";
            var result = await connection.QuerySingleOrDefaultAsync<int>(cmd);
            if (result == null)
            {
                return new Response<int>("Result is null", HttpStatusCode.NotFound);
            }
            return new Response<int>(result, "All worked");
        }
    }

    public async Task<Response<int>> GetBooksWithRasrochka()
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"select b.* from Borrowings br
                                join Books b on b.bookId = br.bookId
                                where br.returnDate > br.DueDate";
            var result = await connection.ExecuteScalarAsync<int>(cmd);
            if (result == null)
            {
                return new Response<int>("Result is null", HttpStatusCode.NotFound);
            }
            return new Response<int>(result, "All worked");
        }
    }

    public async Task<Response<List<Borrowings>>> GetMembersWhoPay()
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"select  distinct m.MemberId, m.FullName
                                    from Members m
                                    join Borrowings b ON r.Id = b.ReaderId
                                    where b.ReturnDate > b.DueDate
                                    and b.Fine > 0;
                                ";
            var result = await connection.QueryAsync<Borrowings>(cmd);
            if (result == null)
            {
                return new Response<List<Borrowings>>("Result is null", HttpStatusCode.NotFound);
            }
            return new Response<List<Borrowings>>(result.ToList(), "All worked");
        }
    }
}