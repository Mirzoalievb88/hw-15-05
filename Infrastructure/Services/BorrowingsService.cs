using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Npgsql;
using Dapper;

namespace Infrastructure.Services;

public class BorrowingsService : IBorrowingsService
{
    private readonly DataContext context = new DataContext();

    public async Task<int> CreateBorrowingsAsync(Borrowings borrowings)
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"insert into Borrowings(BookId, MemberId, BorrowDate, DueDate, ReturnDate, Fine)
                                values (@BookId, @MemberId, @BorrowDate, @DueDate, @ReturnDate, @Fine)";
            var result = await connection.ExecuteAsync(cmd, borrowings);
            return result;
        }
    }

    public async Task<List<Borrowings>> GetBorrowingMemberByIdAsync(int Id)
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"select b.* from Borrowings b
                                where MemberId = @Id";
            var result = await connection.QueryAsync<Borrowings>(cmd, @Id = Id);
            return result.ToList();
        }
    }

    // public async Task<List<Borrowings>> GetBorrowingsAsync()
    // {
    //     using (var connection = await context.GetConnectionAsync())
    //     {
    //         connection.Open();
    //         var cmd = @$"select * from Borrowings";
    //         var result = await connection.QueryAsync<Borrowings>(cmd);
    //         return result.ToList();
    //     }
    // }

    // public async Task<List<Borrowings>> GetBorrowingsWithFiltre()
    // {
    //     using (var connection = await context.GetConnectionAsync())
    //     {
    //         connection.Open();
    //         var cmd = @$"select m.* from Borrowings b
    //                             join Members m on m.MemberId = b.MemberId
    //                             where DueDate < ReturnDate";
    //         var result = await connection.QueryAsync<Borrowings>(cmd);
    //         return result.ToList();
    //     }
    // }
}