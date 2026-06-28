using CodeVaultAPI.Models.Data;
using CodeVaultMVC.Models;
using CodeVaultMVC.Models.MVC_Tables;
using CodeVaultMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CodeVaultMVC.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Users> _userManager;
        private readonly HttpClient _client = new HttpClient();
        private readonly string _apiBase = "https://localhost:7000/api/";

        public TasksController(ApplicationDbContext context, UserManager<Users> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var currentUserId = user?.Id;
            var taskList = await _context.Tasks.Where(t => t.UserId == currentUserId).ToListAsync();

            var projects = JsonConvert.DeserializeObject<List<Projects>>(
                await _client.GetStringAsync(_apiBase + "Projects/GetProjects")) ?? new();
            var developers = JsonConvert.DeserializeObject<List<Developers>>(
                await _client.GetStringAsync(_apiBase + "Developers/GetDevelopers")) ?? new();
            var technologies = JsonConvert.DeserializeObject<List<Technologies>>(
                await _client.GetStringAsync(_apiBase + "Technologies/GetTechnologies")) ?? new();

            var result = taskList.Select(t => new TaskListVM
            {
                TaskID = t.TaskID,
                TaskName = t.TaskName,
                Description = t.Description,
                Status = t.Status,
                ProjectName = projects.FirstOrDefault(p => p.ProjeID == t.ProjectID)?.ProjectName,
                DeveloperName = developers.FirstOrDefault(d => d.DeveloperID == t.DeveloperID)?.FullName,
                TechnologyName = technologies.FirstOrDefault(x => x.TechnologyID == t.TechnologyID)?.TechnologyName
            }).ToList();

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View(await BuildFormVM(new Tasks()));
        }

        [HttpPost]
        public async Task<IActionResult> Create(TaskFormVM vm)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            vm.Task.UserId = user.Id;
            if (string.IsNullOrEmpty(vm.Task.Status)) vm.Task.Status = "Bekliyor";

            // Geliştirici profilini API'den sorgula
            try
            {
                var response = await _client.GetAsync(_apiBase + $"Developers/GetDeveloperByEmail/{Uri.EscapeDataString(user.Email)}");
                if (response.IsSuccessStatusCode)
                {
                    var devJson = await response.Content.ReadAsStringAsync();
                    var dev = JsonConvert.DeserializeObject<Developers>(devJson);
                    if (dev != null)
                    {
                        vm.Task.DeveloperID = dev.DeveloperID;
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Sistemde e-posta adresinizle eşleşen bir geliştirici profili bulunamadı. Lütfen yöneticinizle iletişime geçin.");
                    vm = await BuildFormVM(vm.Task);
                    return View(vm);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Geliştirici profili doğrulanırken API hatası oluştu: {ex.Message}");
                vm = await BuildFormVM(vm.Task);
                return View(vm);
            }

            ModelState.Remove("Task.UserId");
            ModelState.Remove("Task.DeveloperID");

            if (ModelState.IsValid)
            {
                _context.Tasks.Add(vm.Task);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            vm = await BuildFormVM(vm.Task);
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var currentUserId = user?.Id;
            var task = await _context.Tasks.FindAsync(id);
            if (task == null || task.UserId != currentUserId) return NotFound();
            return View(await BuildFormVM(task));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, TaskFormVM vm)
        {
            if (id != vm.Task.TaskID) return BadRequest();

            var user = await _userManager.GetUserAsync(User);
            var currentUserId = user?.Id;
            var existing = await _context.Tasks.FindAsync(id);
            if (existing == null || existing.UserId != currentUserId) return NotFound();

            ModelState.Remove("Task.UserId");
            ModelState.Remove("Task.DeveloperID");

            if (!ModelState.IsValid)
            {
                vm = await BuildFormVM(vm.Task);
                return View(vm);
            }

            existing.TaskName = vm.Task.TaskName;
            existing.Description = vm.Task.Description;
            existing.Status = vm.Task.Status;
            existing.ProjectID = vm.Task.ProjectID;
            existing.TechnologyID = vm.Task.TechnologyID;

            // DeveloperID'yi API'den tekrar doğrula ve ata
            try
            {
                var response = await _client.GetAsync(_apiBase + $"Developers/GetDeveloperByEmail/{Uri.EscapeDataString(user.Email)}");
                if (response.IsSuccessStatusCode)
                {
                    var devJson = await response.Content.ReadAsStringAsync();
                    var dev = JsonConvert.DeserializeObject<Developers>(devJson);
                    if (dev != null)
                    {
                        existing.DeveloperID = dev.DeveloperID;
                    }
                }
            }
            catch {}

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var currentUserId = user?.Id;
            var task = await _context.Tasks.FindAsync(id);
            if (task == null || task.UserId != currentUserId) return NotFound();
            return View(task);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var currentUserId = user?.Id;
            var task = await _context.Tasks.FindAsync(id);
            if (task == null || task.UserId != currentUserId) return BadRequest();

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private async Task<TaskFormVM> BuildFormVM(Tasks task)
        {
            var vm = new TaskFormVM { Task = task };
            try
            {
                vm.Projects = JsonConvert.DeserializeObject<List<Projects>>(
                    await _client.GetStringAsync(_apiBase + "Projects/GetProjects")) ?? new();
                vm.Developers = JsonConvert.DeserializeObject<List<Developers>>(
                    await _client.GetStringAsync(_apiBase + "Developers/GetDevelopers")) ?? new();
                vm.Technologies = JsonConvert.DeserializeObject<List<Technologies>>(
                    await _client.GetStringAsync(_apiBase + "Technologies/GetTechnologies")) ?? new();
            }
            catch { }
            return vm;
        }
    }
}