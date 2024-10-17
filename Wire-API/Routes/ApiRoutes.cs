namespace Wire.Routes;
public static class ApiRoutes
{
    public const string Version = "v1";
    public const string Base = "api" + "/" + Version;

    public static class Project
    {
        public const string GetProjects = Base + "/project";
        public const string GetProject = Base + "/project/{id}";
        public const string CreateProject = Base + "/project";
        public const string UpdateProject = Base + "/project/{id}";
        public const string DeleteProject = Base + "/project/{id}";

    }

    public static class Document
    {
        public const string GetDocuments = Base + "/document";
        public const string GetDocument = Base + "/document/{id}";
        public const string CreateDocument = Base + "/document";
        public const string UpdateDocument = Base + "/document/{id}";
        public const string DeleteDocument = Base + "/document/{id}";
    }

    public static class ContentGeneration
    {
        public const string GenerateText = Base + "/generate/text";
    }
}