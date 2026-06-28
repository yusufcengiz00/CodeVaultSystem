using CodeVaultAPI.Models;
using CodeVaultAPI.Models.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeVaultAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TechnologiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TechnologiesController(ApplicationDbContext dbContext)
        {
            this._context = dbContext;
        }



        [HttpGet]
        [Route("GetTechnologies")]
        public async Task<IEnumerable<Technologies>> GetTechnologies()
        {
            return await _context.Technologies.ToListAsync();
        }

        [HttpGet]
        [Route("GetTechnologiesById/{id}")]
        public async Task<IActionResult> GetTechnologiesById(int id)
        {
            var technology = await _context.Technologies.FindAsync(id);
            if (technology == null) return NotFound();

            return Ok(technology);
        }

        [HttpPost]
        [Route("AddTechnologies")]
        public async Task<Technologies> AddTechnologies(Technologies technologies)
        {
            _context.Add(technologies);
            await _context.SaveChangesAsync();
            return technologies;
        }

        [HttpPut]
        [Route("UpdateTechnologies/{id}")]
        public async Task<Technologies> UpdateTechnologies(Technologies technologies)
        {
            _context.Update(technologies);
            await _context.SaveChangesAsync();
            return technologies;
        }

        [HttpDelete]
        [Route("DeleteTechnologies/{id}")]
        public async Task<IActionResult> DeleteTechnologies(int id)
        {
            var technology = await _context.Technologies.FindAsync(id);
            if (technology == null)
            {
                return NotFound($"ID'si {id} olan teknoloji bulunamadı.");
            }

            _context.Technologies.Remove(technology);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Başarıyla silindi." });
        }
    }
}
