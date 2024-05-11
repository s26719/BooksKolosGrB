namespace KolosGrB.DTOs;

public class BookWithAuthorsDto
{
    public int id { get; set; }
    public string title { get; set; }
    public List<string> authors { get; set; }
}