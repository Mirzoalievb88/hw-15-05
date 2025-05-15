using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IMemberService
{
    Task<int> CreateMemberAsync(Members members);
    Task<List<Members>> GetMembersAsync();
    Task<int> UpdateMembersAsync(Members members);
    Task<int> DeleteMembersByIdAsync(int Id);
    // Task<List<Members>> GetMemberByIdAsync(int Id);
}