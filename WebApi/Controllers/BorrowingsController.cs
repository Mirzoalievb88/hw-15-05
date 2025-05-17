using Domain.Entities;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]


public class BorrowingsController
{
    private BorrowingsService borrService = new BorrowingsService();

    [HttpGet("Id")]
    public async Task<string> ReturnBookAsync(int BorrowingId)
    {
        return await borrService.ReturnBookAsync(BorrowingId);
    }
    
    
    [HttpPost]
    public async Task<string> CreateBorrowingAsync(Borrowings borrowings)
    {
        return await borrService.CreateBorrowingAsync(borrowings);
    }

    [HttpGet]
    public async Task<List<Borrowings>> GetAllBorrowings()
    {
        var result = await borrService.GetAllBorrowings();
        return result;
    }

    [HttpGet("By memberId")]
    public async Task<List<Borrowings>> GetBorrowingsById(int memberId)
    {
        var result = await borrService.GetBorrowingsById(memberId);
        return result;
    }

    [HttpGet("Count")]
    public async Task<int> GetCountOfBorrowings()
    {
        return await borrService.GetCountOfBorrowings();
    }

    [HttpGet("Avg")]
    public async Task<int> GetAvgSumForLate()
    {
        return await borrService.GetAvgSumForLate();
    }

    [HttpGet("NoRturned")]
    public async Task<List<Borrowings>> GetNotReturnedBooks()
    {
        return await borrService.GetNotReturnedBooks();
    }

    [HttpGet("NoCopiesBooks")]
    public async Task<List<Borrowings>> GetBooksWithoutCopies()
    {
        return await borrService.GetBooksWithoutCopies();
    }

    [HttpGet("BooksWithoutGeting")]
    public async Task<int> GetBooksWithoutGeting()
    {
        return await borrService.GetBooksWithoutGeting();
    }
}