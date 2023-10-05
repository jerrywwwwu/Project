using Microsoft.EntityFrameworkCore;
using Project_MVC.Models;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ProjectDataBaseContext>(options => 
options.UseSqlServer(builder.Configuration.GetConnectionString("ProjectDataBaseContext")));


//�W�[session �\��
builder.Services.AddSession();

var app = builder.Build();
//�Х�session �\��
app.UseSession();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Home_Leader}/{id?}");

app.Run();
