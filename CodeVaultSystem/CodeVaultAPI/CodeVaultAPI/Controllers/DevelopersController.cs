using CodeVaultAPI.Models;
using CodeVaultAPI.Models.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeVaultAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevelopersController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public DevelopersController(ApplicationDbContext dbContext)
        {
            this._context = dbContext;
        }



        [HttpGet]
        [Route("GetDevelopers")]
        public async Task<IEnumerable<Developers>> GetDevelopers()
        {
            return await _context.Developers.ToListAsync();
        }

        [HttpGet]
        [Route("GetDevelopersById/{id}")]
        public async Task<IActionResult> GetDevelopersById(int id)
        {
            var developer = await _context.Developers.FindAsync(id);
            if (developer == null) return NotFound();

            return Ok(developer);
        }

        [HttpGet]
        [Route("GetDeveloperByEmail/{email}")]
        public async Task<IActionResult> GetDeveloperByEmail(string email)
        {
            var developer = await _context.Developers.FirstOrDefaultAsync(d => d.EMail.ToLower() == email.ToLower());
            if (developer == null) return NotFound();

            return Ok(developer);
        }

        [HttpPost]
        [Route("AddDevelopers")]
        public async Task<Developers> AddDevelopers(Developers developers)
        {
            _context.Add(developers);
            await _context.SaveChangesAsync();
            return developers;
        }

        [HttpPut]
        [Route("UpdateDevelopers/{id}")]
        public async Task<Developers> UpdateDevelopers(Developers developers)
        {
            _context.Update(developers);
            await _context.SaveChangesAsync();
            return developers;
        }

        [HttpDelete]
        [Route("DeleteDevelopers/{id}")]
        public async Task<IActionResult> DeleteDevelopers(int id)
        {
            var developer = await _context.Developers.FindAsync(id);
            if (developer == null)
            {
                return NotFound($"ID'si {id} olan geliştirici bulunamadı.");
            }

            _context.Developers.Remove(developer);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Başarıyla silindi." });
        }
    }

}
