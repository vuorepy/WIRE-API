using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Wire.Models;

public class Project
{

    [JsonProperty(PropertyName = "id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [JsonProperty(PropertyName = "name")]
    public required string Name { get; set; }
}