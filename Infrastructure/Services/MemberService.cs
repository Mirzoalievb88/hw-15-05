using Domain.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Data;
using Npgsql;
using Dapper;

namespace Infrastructure.Services;

public class MemberService : IMemberService
{
    private readonly DataContext context = new DataContext();

    public async Task<int> CreateMemberAsync(Members members)
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"insert into Members(FullName, Phone, Email, MemberShipDate)
                                values (@FullName, @Phone, @Email, @MemberShipDate)";
            var result = await connection.ExecuteAsync(cmd, members);
            return result;
        }
    }


    public async Task<List<Members>> GetMembersAsync()
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"select * from Members";
            var result = await connection.QueryAsync<Members>(cmd);
            return result.ToList();
        }
    }

    public async Task<int> UpdateMembersAsync(Members members)
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"Update Members
                                set FullName = @FullName, Phone = @Phone, Email = @Email, MemberShipDate = @MemberShipDate";
            var result = await connection.ExecuteAsync(cmd, members);
            return result;
        }
    }
    public async Task<int> DeleteMembersByIdAsync(int Id)
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"Delete from Members
                                Where Id = @Id";
            var result = await connection.ExecuteAsync(cmd);
            return result;
        }
    }

    public async Task<List<Members>> GetMemberByIdAsync(int Id)
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"select * from Members
                                where MemberId = @Id";
            var result = await connection.QueryAsync<Members>(cmd, @Id = Id);
            return result.ToList();
        }
    }

    public async Task<List<Members>> GetTheActiveMember()
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"SELECT m.*
                                FROM Members m
                                JOIN Borrowings b ON m.MemberId = b.MemberId
                                GROUP BY m.MemberId
                                ORDER BY COUNT(b.BorrowingId) DESC
                                LIMIT 1;";
            var result = await connection.QueryAsync<Members>(cmd);
            return result.ToList();
        }
    }
}