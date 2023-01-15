using Database;
using ForkHierarchy.Core.Models;
using ForkHierarchy.Server.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ForkHierarchy.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class QueuedRepositoriesController : ControllerBase
{
    private readonly ForkHierarchyContext _dbContext;

    public QueuedRepositoriesController(ForkHierarchyContext dbContext)
    {
        _dbContext = dbContext;
    }

    // GET: api/<QueuedRepositoriesController>
    [HttpGet("{fullName}")]
    [ProducesResponseType(typeof(QueuedRepository), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetQueuedRepositoryAsync(string fullName)
    {
        var queuedRepository = await _dbContext.QueuedRepositories.FirstOrDefaultAsync(x => $"{x.Owner}/{x.Name}" == fullName);
        if (queuedRepository.ToDto() is not { } dto)
            return NotFound();

        return Ok(dto);
    }

    // POST api/<QueuedRepositoriesController>
    [HttpPost("{owner}/{repo}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    public IActionResult CreateQueuedRepository(string owner, string repo)
    {
        /*
        This is allowed: A refresh of an old dataset
        TODO: Add Refresh Time?

        if (_dbContext.GitHubRepositories.Any(x => x.FullName == $"{owner}/{repo}"))
            return Conflict();
        */
        if (_dbContext.QueuedRepositories.Any(x => x.Owner == owner && x.Name == repo))
            return Conflict();

        var queueItem = new Database.Models.QueuedRepository()
        {
            Owner = owner,
            Name = repo,
            AddedAt = DateTime.UtcNow
        };
        _dbContext.QueuedRepositories.Add(queueItem);
        _dbContext.SaveChanges();
        return Ok();
    }

    /*
    // DELETE api/<QueuedRepositoriesController>/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(string fullName)
    {
        var queuedRepository = await _dbContext.QueuedRepositories.FirstOrDefaultAsync(x => $"{x.Owner}/{x.Name}" == fullName);
        if (queuedRepository is null)
            return NotFound();

        _dbContext.QueuedRepositories.Remove(queuedRepository);

        return Ok();
    }
    */
}
