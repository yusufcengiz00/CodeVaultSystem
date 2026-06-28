using CodeVaultAPI.Models.Data;
using CodeVaultMVC.Models.MVC_Tables;
using Microsoft.AspNetCore.Identity; // IdentityRole için gerekli
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// excel ve pdf kütüphanelerinin lisans tanımlamaları
OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// ADIM 1 GÜNCELLEMESİ: .AddRoles<IdentityRole>() eklendi
builder.Services.AddIdentity<Users, IdentityRole>(options => {
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Authentication her zaman Authorization'dan önce gelmelidir
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=ChooseRole}/{id?}");

// --- ADIM 3: ROL VE ADMIN ATAMA ---
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Users>>();

    // 1. Admin rolünü oluştur
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    // 2. Admin kullanıcısını ata ve oluştur
    var adminEmail = "admin@admin.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new Users
        {
            FullName = "Admin",
            Email = adminEmail,
            UserName = adminEmail,
            EmailConfirmed = true
        };
        var createResult = await userManager.CreateAsync(adminUser, "Admin123*");
        if (createResult.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
    else
    {
        // Şifreyi Admin123* olarak güncelle/sıfırla
        if (await userManager.HasPasswordAsync(adminUser))
        {
            await userManager.RemovePasswordAsync(adminUser);
        }
        await userManager.AddPasswordAsync(adminUser, "Admin123*");

        // Kullanıcı zaten Admin rolünde değilse ekle
        if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}

app.Run();