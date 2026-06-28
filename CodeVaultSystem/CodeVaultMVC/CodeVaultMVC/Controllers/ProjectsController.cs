using CodeVaultMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CodeVaultMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProjectsController : Controller
    {
        private readonly HttpClient _client = new HttpClient();
        private readonly string _api = "https://localhost:7000/api/Projects/";

        public async Task<IActionResult> Index()
        {
            var json = await _client.GetStringAsync(_api + "GetProjects");
            var list = JsonConvert.DeserializeObject<List<Projects>>(json) ?? new();
            return View(list);
        }

        [HttpGet]
        public IActionResult Create() => View(new Projects());

        [HttpPost]
        public async Task<IActionResult> Create(Projects model)
        {
            if (!ModelState.IsValid) return View(model);
            var content = new StringContent(JsonConvert.SerializeObject(model), System.Text.Encoding.UTF8, "application/json");
            await _client.PostAsync(_api + "AddProjects", content);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var json = await _client.GetStringAsync(_api + $"GetProjectsById/{id}");
            var model = JsonConvert.DeserializeObject<Projects>(json);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Projects model)
        {
            if (!ModelState.IsValid) return View(model);
            var content = new StringContent(JsonConvert.SerializeObject(model), System.Text.Encoding.UTF8, "application/json");
            await _client.PutAsync(_api + $"UpdateProjects/{model.ProjeID}", content);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var json = await _client.GetStringAsync(_api + $"GetProjectsById/{id}");
            var model = JsonConvert.DeserializeObject<Projects>(json);
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _client.DeleteAsync(_api + $"DeleteProjects/{id}");
            return RedirectToAction("Index");
        }
    }
}