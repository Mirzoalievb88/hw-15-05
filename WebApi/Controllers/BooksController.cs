using Domain.Entities;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]


public class BooksController(BooksService bookService)
{   
    [HttpGet]
    public async Task<List<Books>> GetBooksAsync()
    {
        var result = await bookService.GetBooksAsync();
        return result;
    }

    [HttpGet("{id}")]
    public async Task<Books?> GetBookAsyncById(int id)
    {
        var result = await bookService.GetBookByIdAsync(id);
        return result;
    }

    [HttpPost]
    public async Task<int> CreateBookAsync(Books books)
    {
        var result = await bookService.CreateBookAsync(books);
        return result;
    }
    [HttpPut]
    public async Task<int> UpdateBookAsync(Books books)
    {
        var result = await bookService.UpdateBookAsync(books);
        return result;
    }
    [HttpDelete]
    public async Task<int> DeleteBookAsync(int Id)
    {
        var result = await bookService.DeleteBookAsync(Id);
        return result;
    }

    [HttpGet("Popular Book")]
    public async Task<List<Books>> GetThePopularBook()
    {
        var result = await bookService.GetThePopularBook();
        return result;
    }

    [HttpGet("BooksAndCounts")]
    public async Task<List<Books>> GetBooksAndBorrowingsCount()
    {
        return await bookService.GetBooksAndBorrowingsCount();
    }
}