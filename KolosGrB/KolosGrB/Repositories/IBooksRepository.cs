using KolosGrB.DTOs;

namespace KolosGrB.Repositories;

public interface IBooksRepository
{
    Task<BookWithAuthorsDto> GetBooksAsync(int id);
    Task<int> AddBookWithAuthorsAsync(BookToAddDto bookToAddDto);

}