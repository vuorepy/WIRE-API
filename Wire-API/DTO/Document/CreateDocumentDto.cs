namespace Wire.DTO.Document
{
    public class CreateDocumentDto
    {
        public Guid ProjectId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}