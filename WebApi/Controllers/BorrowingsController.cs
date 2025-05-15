using Domain.Entities;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]


public class BorrowingsController
{
    private BorrowingsService borrService = new BorrowingsService();

    [HttpGet]
    public async Task<List<Borrowings>> GetBorrowingMemberByIdAsync(int Id)
    {
        var result = await borrService.GetBorrowingMemberByIdAsync(Id);
        return result;
    }

    [HttpPost]
    public async Task<int> CreateBorrowingsAsync(Borrowings borrowings)
    {
        var result = await borrService.CreateBorrowingsAsync(borrowings);
        return result;
    }

    // [HttpGet]
    // public async Task<List<Borrowings>> GetBorrowingsAsync()
    // {
    //     var result = await borrService.GetBorrowingsAsync();
    //     return result;
    // }

    // [HttpGet]
    // public async Task<List<Borrowings>> GetBorrowingsWithFiltre()
    // {
    //     var result = await borrService.GetBorrowingsWithFiltre();
    //     return result;
    // }
}