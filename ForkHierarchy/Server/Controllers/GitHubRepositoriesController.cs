using ForkHierarchy.Core.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ForkHierarchy.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GitHubRepositoriesController : ControllerBase
{

    // GET api/<GitHubRepositoriesController>/5
    [HttpGet("{id}")]
    public GitHubRepository Get(int id)
    {
        return null!;
    }

    // POST api/<GitHubRepositoriesController>
    [HttpPost]
    public void Post([FromBody] string value)
    {
    }

    // PUT api/<GitHubRepositoriesController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/<GitHubRepositoriesController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}
