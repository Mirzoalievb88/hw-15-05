using Domain.DTO;
using Domain.Entities;
using Domain.ApiResponse;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]


public class BorrowingsController(BorrowingsService borrService)
{
    [HttpGet("Id")]
    public async Task<Response<string>> ReturnBookAsync(int BorrowingId)
    {
        return await borrService.ReturnBookAsync(BorrowingId);
    }


    [HttpPost]
    public async Task<Response<string>> CreateBorrowingAsync(Borrowings borrowings)
    {
        return await borrService.CreateBorrowingAsync(borrowings);
    }

    [HttpGet]
    public async Task<Response<List<Borrowings>>> GetAllBorrowings()
    {
        var result = await borrService.GetAllBorrowings();
        return result;
    }

    [HttpGet("By memberId")]
    public async Task<Response<List<Borrowings>>> GetBorrowingsById(int memberId)
    {
        var result = await borrService.GetBorrowingsById(memberId);
        return result;
    }

    [HttpGet("Count")]
    public async Task<Response<int>> GetCountOfBorrowings()
    {
        return await borrService.GetCountOfBorrowings();
    }

    [HttpGet("Avg")]
    public async Task<Response<int>> GetAvgSumForLate()
    {
        return await borrService.GetAvgSumForLate();
    }

    [HttpGet("NoRturned")]
    public async Task<Response<List<Borrowings>>> GetNotReturnedBooks()
    {
        return await borrService.GetNotReturnedBooks();
    }

    [HttpGet("NoCopiesBooks")]
    public async Task<Response<List<Borrowings>>> GetBooksWithoutCopies()
    {
        return await borrService.GetBooksWithoutCopies();
    }

    [HttpGet("BooksWithoutGeting")]
    public async Task<Response<string>> GetBooksWithoutGeting()
    {
        return await borrService.GetBooksWithoutGeting();
    }

    [HttpGet("CountBoorAndMem")]
    public async Task<Response<int>> GetCountOfBorrowingsWithMember()
    {
        return await borrService.GetCountOfBorrowingsWithMember();
    }

    [HttpGet("Obmanwik N1")]
    public async Task<Response<ReaderDto>> GetFirstReaderWithOverdueAsync()
    {
        return await borrService.GetFirstReaderWithOverdueAsync();
    }

    [HttpGet("Top Members")]
    public async Task<Response<List<Borrowings>>> GetTopFiveMembers()
    {
        return await borrService.GetTopFiveMembers();
    }

    [HttpGet("AllPenaltyPerDay")]
    public async Task<Response<int>> GetAllWtraff()
    {
        return await borrService.GetAllWtraff();
    }

    [HttpGet("MemberWhoPayFine")]
    public async Task<Response<List<Borrowings>>> GetMembersWhoPay()
    {
        return await borrService.GetMembersWhoPay();
    }
}