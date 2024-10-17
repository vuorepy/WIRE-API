using Bogus;
using Wire.DTO.Project;
using Wire.Models;

namespace Wire.Tests.Fakers;

public class ProjectFakes : Faker<Project>
{
    public ProjectFakes()
    {
        RuleFor(x => x.Id, f => f.Random.Guid());
        RuleFor(x => x.Name, f => f.Commerce.ProductName());
    }
}

public class CreateProjectDtoFakes : Faker<CreateProjectDto>
{
    public CreateProjectDtoFakes()
    {
        RuleFor(x => x.Name, f => f.Commerce.ProductName());
    }
}

public class UpdateProjectDtoFakes : Faker<UpdateProjectDto>
{
    public UpdateProjectDtoFakes()
    {
        RuleFor(x => x.Name, f => f.Commerce.ProductName());
    }
}