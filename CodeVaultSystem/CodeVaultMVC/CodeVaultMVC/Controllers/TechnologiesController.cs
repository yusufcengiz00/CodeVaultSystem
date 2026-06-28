using CodeVaultMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CodeVaultMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TechnologiesController : Controller
    {
        private readonly HttpClient _client = new HttpClient();
        private readonly string _api = "https://localhost:7000/api/Technologies/";

        public async Task<IActionResult> Index()
        {
            var json = await _client.GetStringAsync(_api + "GetTechnologies");
            var list = JsonConvert.DeserializeObject<List<Technologies>>(json) ?? new();
            return View(list);
        }

        [HttpGet]
        public IActionResult Create() => View(new Technologies());

        [HttpPost]
        public async Task<IActionResult> Create(Technologies model)
        {
            if (!ModelState.IsValid) return View(model);
            var content = new StringContent(JsonConvert.SerializeObject(model), System.Text.Encoding.UTF8, "application/json");
            await _client.PostAsync(_api + "AddTechnologies", content);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var json = await _client.GetStringAsync(_api + $"GetTechnologiesById/{id}");
            var model = JsonConvert.DeserializeObject<Technologies>(json);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Technologies model)
        {
            if (!ModelState.IsValid) return View(model);
            var content = new StringContent(JsonConvert.SerializeObject(model), System.Text.Encoding.UTF8, "application/json");
            await _client.PutAsync(_api + $"UpdateTechnologies/{model.TechnologyID}", content);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var json = await _client.GetStringAsync(_api + $"GetTechnologiesById/{id}");
            var model = JsonConvert.DeserializeObject<Technologies>(json);
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _client.DeleteAsync(_api + $"DeleteTechnologies/{id}");
            return RedirectToAction("Index");
        }
    }
}