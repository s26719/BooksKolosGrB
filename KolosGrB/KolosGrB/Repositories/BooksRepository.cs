using System.Data.SqlClient;
using KolosGrB.DTOs;
using KolosGrB.Exceptions;

namespace KolosGrB.Repositories;

public class BooksRepository : IBooksRepository
{
    private readonly string connectionstring;

    public BooksRepository(IConfiguration configuration)
    {
        connectionstring = configuration.GetConnectionString("Defaultconnection");
    }

    public async Task<BookWithAuthorsDto> GetBooksAsync(int id)
    {
        using var con = new SqlConnection(connectionstring);
        await con.OpenAsync();
        var query = @"Select b.PK, b.title, a.first_name, a.Last_name from books b
                    join books_authors ba ON ba.FK_book = b.PK
                     join authors a ON a.PK = ba.FK_author
                    where b.PK = @idBook";
        using (var cmd = new SqlCommand(query, con))
        {
            cmd.Parameters.AddWithValue("@idBook", id);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {

                BookWithAuthorsDto bookWithAuthorsDto = new()
                {
                    id = int.Parse(reader["PK"].ToString()),
                    title = reader["title"].ToString(),
                    authors = new List<AuthorsDto>()
                };

                do
                {
                    var authorDto = new AuthorsDto()
                    {
                        firstName = reader["first_name"].ToString(),
                        lastName = reader["last_name"].ToString()
                    };
                    bookWithAuthorsDto.authors.Add(authorDto);
                } while (await reader.ReadAsync());
                return bookWithAuthorsDto;
            }
            else
            {
                throw new NotFoundException("nie ma takiej ksiazki");
            }
        }
    }
}