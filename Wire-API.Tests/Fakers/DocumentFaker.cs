
using Bogus;
using Wire.DTO.Document;
using Wire.Models;

public class DocumentFakes : Faker<Document>
{
    public DocumentFakes()
    {
        RuleFor(x => x.Id, f => f.Random.Guid());
        RuleFor(x => x.ProjectId, f => f.Random.Guid());
        RuleFor(x => x.Title, f => f.Commerce.ProductName());
        RuleFor(x => x.Content, f => f.Lorem.Paragraphs());
    }
}

public class UpdateDocumentDtoFakes : Faker<UpdateDocumentDto>
{
    public UpdateDocumentDtoFakes()
    {
        RuleFor(x => x.ProjectId, f => f.Random.Guid());
        RuleFor(x => x.Title, f => f.Commerce.ProductName());
        RuleFor(x => x.Content, f => f.Lorem.Paragraphs());
    }
}

public class CreateDocumentDtoFakes : Faker<CreateDocumentDto>
{
    public CreateDocumentDtoFakes()
    {
        RuleFor(x => x.ProjectId, f => f.Random.Guid());
        RuleFor(x => x.Title, f => f.Commerce.ProductName());
        RuleFor(x => x.Content, f => f.Lorem.Paragraphs());
    }
}