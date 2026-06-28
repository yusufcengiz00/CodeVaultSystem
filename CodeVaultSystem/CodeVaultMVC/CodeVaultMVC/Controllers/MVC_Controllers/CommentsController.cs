using CodeVaultAPI.Models.Data;
using CodeVaultMVC.Models.MVC_Tables;
using CodeVaultMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeVaultMVC.Controllers
{
    [Authorize]
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Users> _userManager;

        public CommentsController(ApplicationDbContext context, UserManager<Users> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var currentUserId = user?.Id;
            var comments = await _context.Comments.Where(c => c.UserId == currentUserId).ToListAsync();
            var tasks = await _context.Tasks.Where(t => t.UserId == currentUserId).ToListAsync();
            var users = await _userManager.Users.ToListAsync();

            var result = comments.Select(c => new CommentListVM
            {
                CommentID = c.CommentID,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                UserName = users.FirstOrDefault(u => u.Id == c.UserId)?.UserName ?? "Bilinmiyor",
                TaskName = tasks.FirstOrDefault(t => t.TaskID == c.TaskID)?.TaskName ?? "Bulunamadı"
            }).ToList();

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(await BuildFormVM(new Comments(), user?.Id));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CommentFormVM vm)
        {
            var user = await _userManager.GetUserAsync(User);
            var currentUserId = user?.Id;

            // Görev kontrolü: Görev kullanıcıya mı ait?
            var targetTask = await _context.Tasks.FindAsync(vm.Comment.TaskID);
            if (targetTask == null || targetTask.UserId != currentUserId)
            {
                ModelState.AddModelError("Comment.TaskID", "Geçersiz görev seçimi! Sadece kendi görevlerinize yorum yazabilirsiniz.");
            }

            vm.Comment.UserId = currentUserId;
            vm.Comment.CreatedAt = DateTime.Now;

            ModelState.Remove("Comment.UserId");

            if (ModelState.IsValid)
            {
                _context.Comments.Add(vm.Comment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(await BuildFormVM(vm.Comment, currentUserId));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var currentUserId = user?.Id;

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null || comment.UserId != currentUserId) return NotFound();

            return View(await BuildFormVM(comment, currentUserId));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CommentFormVM vm)
        {
            if (id != vm.Comment.CommentID) return BadRequest();

            var user = await _userManager.GetUserAsync(User);
            var currentUserId = user?.Id;

            var existing = await _context.Comments.FindAsync(id);
            if (existing == null || existing.UserId != currentUserId) return NotFound();

            // Görev kontrolü: Seçilen görev kullanıcıya mı ait?
            var targetTask = await _context.Tasks.FindAsync(vm.Comment.TaskID);
            if (targetTask == null || targetTask.UserId != currentUserId)
            {
                ModelState.AddModelError("Comment.TaskID", "Geçersiz görev seçimi! Sadece kendi görevlerinize yorum atayabilirsiniz.");
            }

            ModelState.Remove("Comment.UserId");

            if (!ModelState.IsValid)
                return View(await BuildFormVM(vm.Comment, currentUserId));

            existing.Content = vm.Comment.Content;
            existing.TaskID = vm.Comment.TaskID;

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var currentUserId = user?.Id;

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null || comment.UserId != currentUserId) return NotFound();

            return View(comment);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var currentUserId = user?.Id;

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null || comment.UserId != currentUserId) return BadRequest();

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private async Task<CommentFormVM> BuildFormVM(Comments comment, string currentUserId)
        {
            return new CommentFormVM
            {
                Comment = comment,
                Tasks = await _context.Tasks.Where(t => t.UserId == currentUserId).ToListAsync()
            };
        }
    }
}