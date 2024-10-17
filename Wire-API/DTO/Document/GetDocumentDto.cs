namespace Wire.DTO.Document
{
    public class GetDocumentDto
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}