using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using OnlineShopping.Data;

var builder = WebApplication.CreateBuilder(args);
//we are testing for git
// Add services to the container.
builder.Services.AddControllersWithViews();

// DI for Dbcontext
builder.Services.AddDbContext<ShoppingDbContext>(options =>
	options.UseMySQL(builder.Configuration.GetConnectionString("DevConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.LoginPath = "/User/Login"; // Set the login page URL here
		options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
	});

// Add the session services with the MemorySessionStore
builder.Services.AddSession(options =>
{
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});

builder.Services.AddDistributedMemoryCache(); // Add this line to use the MemorySessionStore

var app = builder.Build();

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
app.UseSession();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=User}/{action=Login}/{id?}");

app.Run();

