using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Npgsql;
using Dapper;
using System.Data.Common;

namespace Infrastructure.Services;

public class BorrowingsService : IBorrowingsService
{
    private readonly DataContext context = new DataContext();

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
}