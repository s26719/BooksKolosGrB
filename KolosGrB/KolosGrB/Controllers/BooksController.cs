using KolosGrB.Services;
using Microsoft.AspNetCore.Mvc;

namespace KolosGrB.Controllers;
[Route("api/books")]
public class BooksController : ControllerBase
{
    private readonly IBooksService _booksService;

    public BooksController(IBooksService booksService)
    {
        _booksService = booksService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllBooks(int id)
    {
        return Ok(await _booksService.GetBooksWithAuthorsByIdAsync(id));
    }
}