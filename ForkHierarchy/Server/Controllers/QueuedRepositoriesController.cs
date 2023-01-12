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
    [HttpGet]
    [ProducesResponseType(typeof(QueuedRepository), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetQueuedRepositoryAsync(string fullName)
    {
        var queuedRepository = await _dbContext.QueuedRepositories.FirstOrDefaultAsync(x => $"{x.Owner}/{x.Name}" == fullName);
        if (queuedRepository.ToDto() is not { } dto)
            return NotFound();

        return Ok(dto);
    }

    // POST api/<QueuedRepositoriesController>
    [HttpPost]
    public void PostQueuedRepository([FromBody] string owner, string repo)
    {
        var queueItem = new Database.Models.QueuedRepository()
        {
            Owner = owner,
            Name = repo,
            AddedAt = DateTime.UtcNow
        };
        _dbContext.QueuedRepositories.Add(queueItem);
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
