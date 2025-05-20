using Domain.Entities;
using Infrastructure.Services;
using Domain.ApiResponse;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]


public class BooksController(BooksService bookService)
{   
    [HttpGet]
    public async Task<Response<List<Books>>> GetBooksAsync()
    {
        var result = await bookService.GetBooksAsync();
        return result;
    }

    [HttpGet("{id}")]
    public async Task<Response<Books>> GetBookAsyncById(int id)
    {
        var result = await bookService.GetBookByIdAsync(id);
        return result;
    }

    [HttpPost]
    public async Task<Response<string>> CreateBookAsync(Books books)
    {
        var result = await bookService.CreateBookAsync(books);
        return result;
    }
    [HttpPut]
    public async Task<Response<string>> UpdateBookAsync(Books books)
    {
        var result = await bookService.UpdateBookAsync(books);
        return result;
    }
    [HttpDelete]
    public async Task<Response<string>> DeleteBookAsync(int Id)
    {
        var result = await bookService.DeleteBookAsync(Id);
        return result;
    }

    [HttpGet("Popular Book")]
    public async Task<Response<List<Books>>> GetThePopularBook()
    {
        var result = await bookService.GetThePopularBook();
        return result;
    }

    [HttpGet("Popular genre")]
    public async Task<Response<string>> GetMostPopularGenre()
    {
        return await bookService.GetMostPopularGenre();
    }

    [HttpGet("BooksAndCounts")]
    public async Task<Response<List<Books>>> GetBooksAndBorrowingsCount()
    {
        return await bookService.GetBooksAndBorrowingsCount();
    }
}