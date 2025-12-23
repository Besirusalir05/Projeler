using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using museat.Data;

var builder = WebApplication.CreateBuilder(args);

// Baðlantý dizesini alýrken hata payýný azaltýyoruz
// Program.cs içindeki eski connectionString satýrýný sil ve bunu yapýþtýr
// Önce baðlantý dizesini alalým
// Baðlantý dizesini al
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Baðlantý dizesi bulunamadý!");

// SQL Server yerine MySql (MariaDB) kullanýyoruz
// UseMySql satýrýný bununla deðiþtir
// Program.cs içinde builder.Services.AddDbContext kýsmýný bul ve þununla deðiþtir:
// UseMySql satýrýný bununla deðiþtirin
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(11, 4, 7)), mysqlOptions =>
    {
        mysqlOptions.EnableRetryOnFailure();
    }));
// Plesk resminde versiyonun 11.4.7 olduðunu gördüm, tam olarak bunu yaz.
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// ROL OLUÞTURMA OTOMASYONU
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        string[] roleNames = { "Producer", "Writer" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
    catch (Exception ex)
    {
        // Eðer veritabaný henüz yoksa uygulama burada çökmesin diye loglanabilir
        Console.WriteLine("Roller oluþturulurken bir hata oluþtu: " + ex.Message);
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();