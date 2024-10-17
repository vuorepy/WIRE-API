
using Newtonsoft.Json;

namespace Wire.Models;

public class Document
{
    [JsonProperty(PropertyName = "id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [JsonProperty(PropertyName = "projectId")]
    public Guid ProjectId { get; set; }

    [JsonProperty(PropertyName = "title")]
    public string Title { get; set; }

    [JsonProperty(PropertyName = "content")]
    public string Content { get; set; }

}