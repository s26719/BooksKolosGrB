using System.Data.SqlClient;
using KolosGrB.DTOs;
using KolosGrB.Exceptions;

namespace KolosGrB.Repositories;

public class BooksRepository : IBooksRepository
{
    private readonly string connectionstring;

    public BooksRepository(IConfiguration configuration)
    {
        connectionstring = configuration.GetConnectionString("DefaultConnection");
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


    public async Task<int> AddBookWithAuthorsAsync(BookToAddDto bookToAddDto)
    {
        using var con = new SqlConnection(connectionstring);
        await con.OpenAsync();

        using var transaction = (SqlTransaction)await con.BeginTransactionAsync();
        try
        {
            int idBook;
            // dodaje ksiazke do bazy i pobieram jej Id
            var query1 = "Insert into books(title) output inserted.PK Values(@title)";
            using (var cmd = new SqlCommand(query1, con, transaction))
            {
                cmd.Parameters.AddWithValue("@title", bookToAddDto.title);
                idBook = (int)await cmd.ExecuteScalarAsync();

            }
            //sprawdzam czy istnieje autor
            foreach (var authorDto in bookToAddDto.authors)
            {
                var query3 = "Select count(*) from authors where first_name = @firstname and last_name = @lastname";
                using (var cmd = new SqlCommand(query3, con, transaction))
                {
                    cmd.Parameters.AddWithValue("@firstname", authorDto.firstName);
                    cmd.Parameters.AddWithValue("@lastname", authorDto.lastName);
                    var idcount = (int)await cmd.ExecuteScalarAsync();
                    if (idcount == 0)
                    {
                        throw new NotFoundException("nie ma takiego aktora");
                    }
                }
                
            }
            // przypisuje autora do ksiazki
            foreach (var authorDto in bookToAddDto.authors)
            {
                var query2 = "Insert into books_authors(FK_book, FK_author) values (@idbook, (SELECT a.PK from authors a where first_name = @firstname and last_name = @lastname))";
                using (var cmd = new SqlCommand(query2, con, transaction))
                {
                    cmd.Parameters.AddWithValue("@idbook", idBook);
                    cmd.Parameters.AddWithValue("@firstname", authorDto.firstName);
                    cmd.Parameters.AddWithValue("@lastname", authorDto.lastName);
                    await cmd.ExecuteNonQueryAsync();

                }
            }

            await transaction.CommitAsync();
            return idBook;
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            Console.WriteLine(e);
            throw;
        }
    }
    
}