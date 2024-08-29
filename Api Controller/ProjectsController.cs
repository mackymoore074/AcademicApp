using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using AcademicApp.Data;

namespace AcademicApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IDistributedCache _cache;

        public ProjectsController(AppDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            string cacheKey = "projects";
            var cachedProjects = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedProjects))
            {
                var projects = JsonConvert.DeserializeObject<IEnumerable<Project>>(cachedProjects);
                return Ok(projects);
            }

            var projectsFromDb = await _context.Projects.ToListAsync();
            var projectsJson = JsonConvert.SerializeObject(projectsFromDb);
            await _cache.SetStringAsync(cacheKey, projectsJson, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
            });

            return Ok(projectsFromDb);
        }
    }
}