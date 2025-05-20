using Domain.ApiResponse;
using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IMemberService
{
    Task<Response<int>> CreateMemberAsync(Members members);
    Task<Response<List<Members>>> GetMembersAsync();
    Task<Response<int>> UpdateMembersAsync(Members members);
    Task<Response<int>> DeleteMembersByIdAsync(int Id);
    Task<Response<List<Members>>> GetMemberByIdAsync(int Id);
    Task<Response<List<Members>>> GetTheActiveMember();
}