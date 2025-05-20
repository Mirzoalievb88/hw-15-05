using Domain.Entities;
using Domain.ApiResponse;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]

public class MembersController(MemberService memService)
{

    [HttpGet]
    public async Task<Response<List<Members>>> GetMembersAsync()
    {
        var result = await memService.GetMembersAsync();
        return result;
    }
    
    [HttpPost]
    public async Task<Response<int>> CreateMemberAsync(Members members)
    {
        var result = await memService.CreateMemberAsync(members);
        return result;
    }

    [HttpPut]
    public async Task<Response<int>> UpdateMemberAsync(Members members)
    {
        var result = await memService.UpdateMembersAsync(members);
        return result;
    }

    [HttpDelete]
    public async Task<Response<int>> DeleteMembersByIdAsync(int Id)
    {
        var result = await memService.DeleteMembersByIdAsync(Id);
        return result;
    }

    [HttpGet("{id}")]
    public async Task<Response<List<Members>>> GetMembersByIdAsync(int Id)
    {
        var result = await memService.GetMemberByIdAsync(Id);
        return result;
    }

    [HttpGet("The Active Member")]
    public async Task<Response<List<Members>>> GetTheActiveMember()
    {
        return await memService.GetTheActiveMember();
    }
}