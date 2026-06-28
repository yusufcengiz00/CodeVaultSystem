using CodeVaultAPI.Models.Data;
using CodeVaultMVC.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CodeVaultMVC.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Users> _userManager;

        public UserController(ApplicationDbContext context, UserManager<Users> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var model = new UserDashboardViewModel
            {
                MyTasks = _context.Tasks.Where(t => t.UserId == user.Id).ToList(),
                MyComments = _context.Comments.Where(c => c.UserId == user.Id).ToList()
            };

            return View(model);
        }
    }
}