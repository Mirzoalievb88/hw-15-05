using Domain.Entities;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]

public class MembersController
{
    private MemberService memService = new MemberService();

    [HttpGet]
    public async Task<List<Members>> GetMembersAsync()
    {
        var result = await memService.GetMembersAsync();
        return result;
    }
    
    [HttpPost]
    public async Task<int> CreateMemberAsync(Members members)
    {
        var result = await memService.CreateMemberAsync(members);
        return result;
    }

    [HttpPut]
    public async Task<int> UpdateMemberAsync(Members members)
    {
        var result = await memService.UpdateMembersAsync(members);
        return result;
    }

    [HttpDelete]
    public async Task<int> DeleteMembersByIdAsync(int Id)
    {
        var result = await memService.DeleteMembersByIdAsync(Id);
        return result;
    }

    [HttpGet("{id}")]
    public async Task<List<Members>> GetMembersByIdAsync(int Id)
    {
        var result = await memService.GetMemberByIdAsync(Id);
        return result;
    }

    [HttpGet("The Active Member")]
    public async Task<List<Members>> GetTheActiveMember()
    {
        return await memService.GetTheActiveMember();
    }
    
}