using KolosGrB.DTOs;
using KolosGrB.Repositories;

namespace KolosGrB.Services;

public class BooksService : IBooksService
{
    private readonly IBooksRepository _booksRepository;

    public BooksService(IBooksRepository booksRepository)
    {
        _booksRepository = booksRepository;
    }

    public async Task<BookWithAuthorsDto> GetBooksWithAuthorsByIdAsync(int id)
    {
        return await _booksRepository.GetBooksAsync(id);
    }

    public async Task<BookWithAuthorsDto> AddBookWithAuthorsAsync(BookToAddDto bookToAddDto)
    {
        var idbook = await _booksRepository.AddBookWithAuthorsAsync(bookToAddDto);
        var result = await _booksRepository.GetBooksAsync(idbook);
        return result;
    }
}