namespace Wire.DTO.Document
{
    public class UpdateDocumentDto
    {
        public Guid ProjectId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}