using Database;
using ForkHierarchy.Core.Models;
using ForkHierarchy.Server.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Net;

namespace ForkHierarchy.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GitHubRepositoriesController : ControllerBase
{
    private readonly ForkHierarchyContext _dbContext;

    public GitHubRepositoriesController(ForkHierarchyContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GitHubRepository), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetGitHubRepositoryByIdAsync(int id)
    {
        var dto = await GetAsync(x => x.Id == id);
        if (dto is null)
            return NotFound();

        return Ok(dto);
    }

    [HttpGet]
    [ProducesResponseType(typeof(GitHubRepository), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetGitHubRepositoryByFullNameAsync(string fullName)
    {
        var dto = await GetAsync(x => x.FullName == fullName);
        if (dto is null)
            return NotFound();

        return Ok(dto);
    }

    private async Task<GitHubRepository?> GetAsync(Expression<Func<Database.Models.GitHubRepository, bool>> predicate)
    {
        var dbo = await _dbContext.GitHubRepositories
            .Include(x => x.Owner)
            .FirstOrDefaultAsync(predicate);

        if (dbo.ToDto() is not { } dto)
            return null;

        AddChildrenFor(dto);

        return dto;
    }

    private void AddChildrenFor(GitHubRepository dto)
    {
        foreach (var child in _dbContext.GitHubRepositories.Where(x => x.ParentId == dto.Id))
        {
            var childDto = child.ToDto()!;
            AddChildrenFor(childDto);
            dto.Children.Add(childDto);
        }
    }
}
