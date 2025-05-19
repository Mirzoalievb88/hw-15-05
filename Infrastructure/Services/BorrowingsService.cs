using Domain.Entities;
using Domain.DTO;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Npgsql;
using Dapper;
using System.Data.Common;
using Domain.DTO;

namespace Infrastructure.Services;

public class BorrowingsService(DataContext context) : IBorrowingsService
{
    public async Task<List<Borrowings>> GetAllBorrowingsAsync()
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            string cmd = @$"select * from Borrowings";
            var result = await connection.QueryAsync<Borrowings>(cmd);
            return result.ToList();
        }
    }
    public async Task<Borrowings?> GetBorrowingByMemberId(int memberId)
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();

            string cmd = $@"Select * from Borrowings Where memberId = @memberId";
            var result = await connection.QueryFirstOrDefaultAsync<Borrowings>(cmd, new { memberId = memberId });
            return result;
        }
    }

    public async Task<string> CreateBorrowingAsync(Borrowings borrowings)
    {
        using (var connection = await context.GetConnectionAsync())
        {
            var bookCommand = @$"select * from books where bookId = @Id";
            var book = await connection.QueryFirstOrDefaultAsync<Books>(bookCommand, new { Id = borrowings.BookId });
            if (bookCommand == null)
            {
                return "Book not found";
            }
            if (book.AvailableCopies <= 0)
            {
                return "Available copies are not available";
            }
            if (borrowings.BorrowDate >= borrowings.DueDate)
            {
                return "Borrowing due date is earlier";
            }
            var borrowingCommand = @"insert into borrowings (bookid, memberId, borrowDate, dueDate)
                                            values (@bookId, @memberId, @borrowDate, @dueDate)";
            var result = await connection.ExecuteAsync(borrowingCommand, borrowings);
            if (result == 0)
            {
                return "Borrowing not created";
            }

            var updateBookCommand = @"update books set availableCopies = availableCopies - 1 
                                                where BookId = @Id";
            await connection.ExecuteAsync(updateBookCommand, new { Id = borrowings.BookId });

            return "Borrowing created";
        }
    }

    public async Task<string> ReturnBookAsync(int BorrowingId)
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var borrowingCommand = @"select * from borrowings where borrowingId = @Id";
            var borrowing = await connection.QueryFirstOrDefaultAsync<Borrowings>(borrowingCommand, new {Id = BorrowingId});
            if (borrowing == null)
            {
                return "Borrowing not found";
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
                return "Borrowing not updated";
            }

            var updateBookCommand = @"update books set availableCopies = availableCopies + 1 where bookId = @Id";
            await connection.ExecuteAsync(updateBookCommand, new { Id = borrowing.BookId });

            return "Borrowing updated";
        }
        
    }

    public async Task<List<Borrowings>> GetAllBorrowings()
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"select * from borrowings";
            var result = await connection.QueryAsync<Borrowings>(cmd);
            return result.ToList();
        }
    }

    public async Task<List<Borrowings>> GetBorrowingsById(int memberId)
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"select b.*, m.* from borrowings
                                join Members m on m.memberId = b.memberId
                                where Id = @Id";
            var result = await connection.QueryAsync<Borrowings>(cmd, new { @Id = memberId });
            return result.ToList();
        }
    }

    public async Task<int> GetCountOfBorrowings()
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"select count(*) from borrowings
                                where returnDate < Duedate";
            var result = await connection.ExecuteScalarAsync<int>(cmd);
            return result;
        }
    }

    public async Task<int> GetAvgSumForLate()
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"SELECT AVG(EXTRACT(DAY FROM (ReturnDate - DueDate)) * 1.5)
                                    FROM Borrowings
                                    WHERE ReturnDate > DueDate;
                                    ";
            var result = await connection.ExecuteScalarAsync<int>(cmd);
            return result;
        }
    }

    public async Task<List<Borrowings>> GetNotReturnedBooks()
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"SELECT b.BorrowingId, b.BookId, b.MemberId, b.BorrowDate, b.DueDate
                                    FROM Borrowings b
                                    WHERE b.ReturnDate IS NULL;
                                    ";
            var result = await connection.QueryAsync<Borrowings>(cmd);
            return result.ToList();
        }
    }

    public async Task<List<Borrowings>> GetBooksWithoutCopies()
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"select b.BookId, b.Title, b.Genre from Borrowings bo
                                join Books b on b.BokkId = bo.BookId
                                where Totalcopies < 2";
            var result = await connection.QueryAsync<Borrowings>(cmd);
            return result.ToList();
        }
    }

    public async Task<int> GetBooksWithoutGeting()
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
            return result;
        }
    }

    public async Task<int> GetCountOfBorrowingsWithMember()
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"select count(b.*), m.* from borrowings b
                                group by m.memberId";
            var result = await connection.ExecuteScalarAsync<int>(cmd);
            return result;
        }
    }

    public async Task<ReaderDto> GetFirstReaderWithOverdueAsync()
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
            return result!;
        }
    }

    public async Task<List<Borrowings>> GetTopFiveMembers()
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"select m.* from Borrowings br
                                join Members m on m.memberId = br.memberId
                                limit 5";
            var result = await connection.QueryAsync<Borrowings>(cmd);
            return result.ToList();
        }
    }

    public async Task<int> GetAllWtraff()
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"select sum(extract(day from (ReturnDate - DueDate)) * 1.0)
                                    from Borrowings
                                    where ReturnDate > DueDate;
                                ";
            var result = await connection.QuerySingleOrDefaultAsync<int>(cmd);
            return result;
        }
    }

    public async Task<int> GetBooksWithRasrochka()
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"select b.* from Borrowings br
                                join Books b on b.bookId = br.bookId
                                where br.returnDate > br.DueDate";
            var result = await connection.ExecuteScalarAsync<int>(cmd);
            return result;
        }
    }

    public async Task<List<Borrowings>> GetMembersWhoPay()
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
            return result.ToList();
        }
    }
}