using KolosGrB.DTOs;

namespace KolosGrB.Services;

public interface IBooksService
{
    Task<BookWithAuthorsDto> GetBooksWithAuthorsByIdAsync(int id);
    Task<BookWithAuthorsDto> AddBookWithAuthorsAsync(BookToAddDto bookToAddDto);
}