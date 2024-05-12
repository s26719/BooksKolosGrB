using KolosGrB.DTOs;
using KolosGrB.Exceptions;
using KolosGrB.Services;
using Microsoft.AspNetCore.Mvc;

namespace KolosGrB.Controllers;
[Route("api/books")]
[ApiController]
public class BooksController : ControllerBase
{
    private readonly IBooksService _booksService;

    public BooksController(IBooksService booksService)
    {
        _booksService = booksService;
    }

    [HttpGet("{id}/authors")]
    public async Task<IActionResult> GetAuthorsForBook(int id)
    {
        try
        {
            var bookWithAuthors = await _booksService.GetBooksWithAuthorsByIdAsync(id);
            return Ok(bookWithAuthors); // Zwraca listę autorów dla konkretnej książki
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Wystąpił błąd: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddBookWithAuthors(BookToAddDto bookToAddDto)
    {
        try
        {
            var blad = await _booksService.AddBookWithAuthorsAsync(bookToAddDto);
            return Ok(blad);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Wystąpił błąd: {ex.Message}");
        }
    }
}