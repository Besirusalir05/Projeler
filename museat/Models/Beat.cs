namespace museat.Models
{
    public class Beat
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Genre { get; set; }
        public decimal Price { get; set; }
        public string? AudioFilePath { get; set; }
        public string? ProducerId { get; set; }
    }
}