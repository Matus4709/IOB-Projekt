using Microsoft.EntityFrameworkCore;
using UserManagmentApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Konfiguracja bazy danych
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 21))));

// Dodanie us³ug do kontenera
builder.Services.AddControllersWithViews();

// Konfiguracja sesji
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Sesja wa¿na przez 30 minut
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Konfiguracja pipeline'u HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Dodanie sesji do middleware
app.UseSession(); // To jest w³aœciwe miejsce dla u¿ywania sesji w middleware

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
