namespace LaunchPad.DTO
{
    // Data Transfer Object for Book
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public int Year { get; set; }
        // Add more fields as needed, but keep it minimal for API consumers
    }
}