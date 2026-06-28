using CodeVaultAPI.Models;
using CodeVaultAPI.Models.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeVaultAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext dbContext)
        {
            this._context = dbContext;
        }



        [HttpGet]
        [Route("GetProjects")]
        public async Task<IEnumerable<Projects>> GetProjects()
        {
            return await _context.Projects.ToListAsync();
        }

        [HttpGet]
        [Route("GetProjectsById/{id}")]
        public async Task<IActionResult> GetProjectsById(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return NotFound();

            return Ok(project);
        }

        [HttpPost]
        [Route("AddProjects")]
        public async Task<Projects> AddProjects(Projects projects)
        {
            _context.Add(projects);
            await _context.SaveChangesAsync();
            return projects;
        }

        [HttpPut]
        [Route("UpdateProjects/{id}")]
        public async Task<Projects> UpdateProjects(Projects projects)
        {
            _context.Update(projects);
            await _context.SaveChangesAsync();
            return projects;
        }

        [HttpDelete]
        [Route("DeleteProjects/{id}")]
        public async Task<IActionResult> DeleteProjects(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound($"ID'si {id} olan proje bulunamadı.");
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Başarıyla silindi." });
        }
    }
}
