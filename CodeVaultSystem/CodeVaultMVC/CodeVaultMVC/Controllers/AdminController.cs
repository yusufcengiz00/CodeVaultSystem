using CodeVaultMVC.Models;
using CodeVaultMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CodeVaultMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly HttpClient _client = new HttpClient();
        private readonly string _apiBase = "https://localhost:7000/api/";

        // API'den dashboard verilerini toplu çeken yardımcı metot
        private async Task<AdminDashboardViewModel> GetDashboardDataAsync()
        {
            var projects = new List<Projects>();
            var technologies = new List<Technologies>();
            var developers = new List<Developers>();

            try
            {
                var projectsJson = await _client.GetStringAsync(_apiBase + "Projects/GetProjects");
                projects = JsonConvert.DeserializeObject<List<Projects>>(projectsJson) ?? new();
            }
            catch {}

            try
            {
                var technologiesJson = await _client.GetStringAsync(_apiBase + "Technologies/GetTechnologies");
                technologies = JsonConvert.DeserializeObject<List<Technologies>>(technologiesJson) ?? new();
            }
            catch {}

            try
            {
                var developersJson = await _client.GetStringAsync(_apiBase + "Developers/GetDevelopers");
                developers = JsonConvert.DeserializeObject<List<Developers>>(developersJson) ?? new();
            }
            catch {}

            return new AdminDashboardViewModel
            {
                Projects = projects,
                Technologies = technologies,
                Developers = developers
            };
        }

        public async Task<IActionResult> Index()
        {
            var model = await GetDashboardDataAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadExcel()
        {
            var data = await GetDashboardDataAsync();

            using (var package = new ExcelPackage())
            {
                // Projeler çalışma sayfası
                var wsProjects = package.Workbook.Worksheets.Add("Projeler");
                
                wsProjects.Cells[1, 1].Value = "Proje ID";
                wsProjects.Cells[1, 2].Value = "Proje Adı";
                wsProjects.Cells[1, 3].Value = "Açıklama";
                wsProjects.Cells[1, 4].Value = "Proje URL";
                wsProjects.Cells[1, 5].Value = "Durum";
                
                using (var range = wsProjects.Cells[1, 1, 1, 5])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                int row = 2;
                foreach (var p in data.Projects)
                {
                    wsProjects.Cells[row, 1].Value = p.ProjeID;
                    wsProjects.Cells[row, 2].Value = p.ProjectName;
                    wsProjects.Cells[row, 3].Value = p.Description;
                    wsProjects.Cells[row, 4].Value = p.ProjectUrl;
                    wsProjects.Cells[row, 5].Value = p.Status;
                    row++;
                }
                
                wsProjects.Cells[wsProjects.Dimension.Address].AutoFitColumns();

                // Teknolojiler çalışma sayfası
                var wsTechs = package.Workbook.Worksheets.Add("Teknolojiler");
                wsTechs.Cells[1, 1].Value = "Teknoloji ID";
                wsTechs.Cells[1, 2].Value = "Teknoloji Adı";
                wsTechs.Cells[1, 3].Value = "Kategori";
                wsTechs.Cells[1, 4].Value = "Versiyon";

                using (var range = wsTechs.Cells[1, 1, 1, 4])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                row = 2;
                foreach (var t in data.Technologies)
                {
                    wsTechs.Cells[row, 1].Value = t.TechnologyID;
                    wsTechs.Cells[row, 2].Value = t.TechnologyName;
                    wsTechs.Cells[row, 3].Value = t.Category;
                    wsTechs.Cells[row, 4].Value = t.Version;
                    row++;
                }
                wsTechs.Cells[wsTechs.Dimension.Address].AutoFitColumns();

                // Geliştiriciler çalışma sayfası
                var wsDevs = package.Workbook.Worksheets.Add("Geliştiriciler");
                wsDevs.Cells[1, 1].Value = "Geliştirici ID";
                wsDevs.Cells[1, 2].Value = "Ad Soyad";
                wsDevs.Cells[1, 3].Value = "E-Posta";
                wsDevs.Cells[1, 4].Value = "GitHub URL";
                wsDevs.Cells[1, 5].Value = "LinkedIn URL";

                using (var range = wsDevs.Cells[1, 1, 1, 5])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                row = 2;
                foreach (var d in data.Developers)
                {
                    wsDevs.Cells[row, 1].Value = d.DeveloperID;
                    wsDevs.Cells[row, 2].Value = d.FullName;
                    wsDevs.Cells[row, 3].Value = d.EMail;
                    wsDevs.Cells[row, 4].Value = d.GithubUrl;
                    wsDevs.Cells[row, 5].Value = d.LinkedinUrl;
                    row++;
                }
                wsDevs.Cells[wsDevs.Dimension.Address].AutoFitColumns();

                var fileContents = package.GetAsByteArray();
                return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "CodeVault_Dashboard.xlsx");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DownloadPdf()
        {
            var data = await GetDashboardDataAsync();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    // Sayfa boyutu, kenar boşlukları ve varsayılan font ayarları
                    page.Size(PageSizes.A4);
                    page.Margin(1.5f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(10));

                    page.Header()
                        .Text("CodeVault Admin Panel Raporu")
                        .SemiBold().FontSize(18).FontColor(Colors.Indigo.Darken3);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Spacing(15);

                            // Projeler tablosu
                            column.Item().Text("Projeler").FontSize(14).Bold().FontColor(Colors.Indigo.Darken1);
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(40);
                                    columns.RelativeColumn(3);
                                    columns.RelativeColumn(5);
                                    columns.RelativeColumn(2);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Background(Colors.Indigo.Lighten5).Padding(5).Text("ID").Bold();
                                    header.Cell().Background(Colors.Indigo.Lighten5).Padding(5).Text("Proje Adı").Bold();
                                    header.Cell().Background(Colors.Indigo.Lighten5).Padding(5).Text("Açıklama").Bold();
                                    header.Cell().Background(Colors.Indigo.Lighten5).Padding(5).Text("Durum").Bold();
                                });

                                foreach (var p in data.Projects)
                                {
                                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(p.ProjeID.ToString());
                                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(p.ProjectName ?? "");
                                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(p.Description ?? "");
                                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(p.Status ?? "");
                                }
                            });

                            column.Item().PageBreak();

                            // Teknolojiler tablosu
                            column.Item().Text("Teknolojiler").FontSize(14).Bold().FontColor(Colors.Indigo.Darken1);
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(40);
                                    columns.RelativeColumn(4);
                                    columns.RelativeColumn(4);
                                    columns.RelativeColumn(2);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Background(Colors.Indigo.Lighten5).Padding(5).Text("ID").Bold();
                                    header.Cell().Background(Colors.Indigo.Lighten5).Padding(5).Text("Teknoloji Adı").Bold();
                                    header.Cell().Background(Colors.Indigo.Lighten5).Padding(5).Text("Kategori").Bold();
                                    header.Cell().Background(Colors.Indigo.Lighten5).Padding(5).Text("Versiyon").Bold();
                                });

                                foreach (var t in data.Technologies)
                                {
                                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(t.TechnologyID.ToString());
                                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(t.TechnologyName ?? "");
                                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(t.Category ?? "");
                                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(t.Version ?? "");
                                }
                            });

                            column.Item().PageBreak();

                            // Geliştiriciler tablosu
                            column.Item().Text("Geliştiriciler").FontSize(14).Bold().FontColor(Colors.Indigo.Darken1);
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(40);
                                    columns.RelativeColumn(4);
                                    columns.RelativeColumn(6);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Background(Colors.Indigo.Lighten5).Padding(5).Text("ID").Bold();
                                    header.Cell().Background(Colors.Indigo.Lighten5).Padding(5).Text("Ad Soyad").Bold();
                                    header.Cell().Background(Colors.Indigo.Lighten5).Padding(5).Text("E-Posta").Bold();
                                });

                                foreach (var d in data.Developers)
                                {
                                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(d.DeveloperID.ToString());
                                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(d.FullName ?? "");
                                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(d.EMail ?? "");
                                }
                            });
                        });

                    // Sayfa numaralandırması
                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Sayfa ");
                            x.CurrentPageNumber();
                            x.Span(" / ");
                            x.TotalPages();
                        });
                });
            });

            var pdfBytes = document.GeneratePdf();
            return File(pdfBytes, "application/pdf", "CodeVault_Dashboard.pdf");
        }
    }
}