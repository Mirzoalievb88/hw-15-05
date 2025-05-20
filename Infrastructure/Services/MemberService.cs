using Domain.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Data;
using Npgsql;
using Dapper;
using Domain.ApiResponse;
using System.Net;

namespace Infrastructure.Services;

public class MemberService(DataContext context) : IMemberService
{
    public async Task<Response<int>> CreateMemberAsync(Members members)
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"insert into Members(FullName, Phone, Email, MemberShipDate)
                                values (@FullName, @Phone, @Email, @MemberShipDate)";
            var result = await connection.ExecuteAsync(cmd, members);
            if (result == null)
            {
                return new Response<int>("Result is null", HttpStatusCode.NotFound);
            }
            return new Response<int>(default, "All worked");
        }
    }


    public async Task<Response<List<Members>>> GetMembersAsync()
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"select * from Members";
            var result = await connection.QueryAsync<Members>(cmd);
            if (result == null)
            {
                return new Response<List<Members>>("Result is null", HttpStatusCode.NotFound);
            }
            return new Response<List<Members>>(result.ToList(), "All worked");
        }
    }

    public async Task<Response<int>> UpdateMembersAsync(Members members)
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"Update Members
                                set FullName = @FullName, Phone = @Phone, Email = @Email, MemberShipDate = @MemberShipDate";
            var result = await connection.ExecuteAsync(cmd, members);
            if (result == null)
            {
                return new Response<int>("Result is null", HttpStatusCode.NotFound);
            }
            return new Response<int>(result, "All worked");
        }
    }

    public async Task<Response<int>> DeleteMembersByIdAsync(int Id)
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"Delete from Members
                                Where Id = @Id";
            var result = await connection.ExecuteAsync(cmd);
            if (result == null)
            {
                return new Response<int>("Result is null", HttpStatusCode.NotFound);
            }
            return new Response<int>(default, "All worked");
        }
    }

    public async Task<Response<List<Members>>> GetMemberByIdAsync(int Id)
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"select * from Members
                                where MemberId = @Id";
            var result = await connection.QueryAsync<Members>(cmd, @Id = Id);
            if (result == null)
            {
                return new Response<List<Members>>("Result is null", HttpStatusCode.NotFound);
            }
            return new Response<List<Members>>(result.ToList(), "All worked");
        }
    }

    public async Task<Response<List<Members>>> GetTheActiveMember()
    {
        using (var connection = await context.GetConnectionAsync())
        {
            connection.Open();
            var cmd = @$"SELECT m.*
                                from Members m
                                join Borrowings b ON m.MemberId = b.MemberId
                                GROUP BY m.MemberId
                                order by COUNT(b.BorrowingId) DESC
                                limit 1;";
            var result = await connection.QueryAsync<Members>(cmd);
            if (result == null)
            {
                return new Response<List<Members>>("Result is null", HttpStatusCode.NotFound);
            }
            return new Response<List<Members>>(result.ToList(), "All worked");
        }
    }
}