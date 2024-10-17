
using Microsoft.AspNetCore.Mvc;
using Wire.Models;
using Wire.Routes;
using Wire.Services;

namespace Wire.Controllers;

using AutoMapper;
using Wire.DTO.Project;

[ApiController]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly IMapper _mapper;

    public ProjectController(IProjectService projectService, IMapper mapper)
    {
        _projectService = projectService;
        _mapper = mapper;
    }

    [HttpGet]
    [Route(ApiRoutes.Project.GetProjects)]
    public async Task<IActionResult> GetProjects()
    {
        var projects = await _projectService.GetAllProjectsAsync();

        if (projects == null)
        {
            return NotFound();
        }

        var projectDtos = _mapper.Map<IEnumerable<GetProjectDto>>(projects);
        return Ok(projectDtos);
    }

    [HttpGet]
    [Route(ApiRoutes.Project.GetProject)]
    public async Task<IActionResult> GetProject(string id)
    {
        var project = await _projectService.GetProjectAsync(id);

        if (project == null)
        {
            return NotFound();
        }

        var projectDto = _mapper.Map<GetProjectDto>(project);
        return Ok(projectDto);
    }

    [HttpPost]
    [Route(ApiRoutes.Project.CreateProject)]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto projectDto)
    {
        var project = _mapper.Map<Project>(projectDto);
        var createdProject = await _projectService.CreateProjectAsync(project);

        if (createdProject == null)
        {
            return BadRequest();
        }

        var createdProjectDto = _mapper.Map<GetProjectDto>(createdProject);
        return Created(ApiRoutes.Project.GetProject.Replace("{id}", createdProjectDto.Id.ToString()), createdProjectDto);
    }

    [HttpPut]
    [Route(ApiRoutes.Project.UpdateProject)]
    public async Task<IActionResult> UpdateProject(string id, [FromBody] UpdateProjectDto projectDto)
    {
        var project = _mapper.Map<Project>(projectDto);
        var updatedProject = await _projectService.UpdateProjectAsync(id, project);

        if (updatedProject == null)
        {
            return BadRequest();
        }

        var updatedProjectDto = _mapper.Map<GetProjectDto>(updatedProject);
        return Ok(updatedProjectDto);
    }

    [HttpDelete]
    [Route(ApiRoutes.Project.DeleteProject)]
    public async Task<IActionResult> DeleteProject(string id)
    {
        var deleted = await _projectService.RemoveProjectAsync(id);

        if (deleted == false)
        {
            return NotFound();
        }

        return NoContent();
    }
}

