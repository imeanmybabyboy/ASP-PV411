using ASP_PV411.Data;
using ASP_PV411.Middleware;
using ASP_PV411.Services.FileExtensionChecker;
using ASP_PV411.Services.FolderName;
using ASP_PV411.Services.Hash;
using ASP_PV411.Services.Kdf;
using ASP_PV411.Services.OTP;
using ASP_PV411.Services.Random;
using ASP_PV411.Services.Salt;
using ASP_PV411.Services.Signature;
using ASP_PV411.Services.Storage;
using ASP_PV411.Services.Timestamp;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


// Реєстрація сервісів
// Зазвичай, реєструються не об'єкти, а класи (типи), об'єкти створюються автомаитчно при першому запиті на інжекцію (ледаче створення)
builder.Services.AddRandom();
builder.Services.AddTimestamp();
builder.Services.AddHash();
builder.Services.AddSalt();
builder.Services.AddKdf();
builder.Services.AddSignature();
builder.Services.AddOtp();
builder.Services.AddFolderName();
builder.Services.AddStorage();
builder.Services.AddChecker();


// сесії - як інструмент збереження даних між запитами
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(15);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Підключення контексту даних
string connectionString = builder.Configuration.GetConnectionString("LocalDb") ?? throw new FileNotFoundException("Connection String Configuration: key not found: LocalDb");
// Для EF-контекстів є свій метод реєстрації - AddDbContext
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(connectionString));

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession();


// Custom middleware
app.UseAuthSession(); // додатковий Middleware


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
