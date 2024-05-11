namespace KolosGrB.DTOs;

public class BookWithAuthorsDto
{
    public int id { get; set; }
    public string title { get; set; }
    public List<AuthorsDto> authors { get; set; }
}