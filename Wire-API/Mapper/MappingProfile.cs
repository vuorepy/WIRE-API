using AutoMapper;
using Wire.DTO.Document;
using Wire.DTO.Project;
using Wire.Models;

namespace Wire.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Project mapping 
            CreateMap<Project, GetProjectDto>();
            CreateMap<CreateProjectDto, Project>();
            CreateMap<UpdateProjectDto, Project>();

            // Document mapping
            CreateMap<Document, GetDocumentDto>();
            CreateMap<CreateDocumentDto, Document>();
            CreateMap<UpdateDocumentDto, Document>();
        }
    }
}