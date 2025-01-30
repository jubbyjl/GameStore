using Microsoft.EntityFrameworkCore;
using GameStore.Data;
using Microsoft.AspNetCore.Identity;
using GameStore.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<GameStoreContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("GameStoreContext") ?? throw new InvalidOperationException("Connection string 'GameStoreContext' not found.")));

builder.Services
    .AddIdentity<StoreUser,IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<GameStoreContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

// Generate lowercase URLs
builder.Services.Configure<RouteOptions>(options =>
{
   options.LowercaseUrls = true;
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
