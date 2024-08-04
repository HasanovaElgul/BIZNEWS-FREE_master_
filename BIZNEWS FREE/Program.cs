using BIZNEWS_FREE.Data;
using BIZNEWS_FREE.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//builder.Services.AddRazorPages().AddRazorRuntimeCompilation();     //bild etmeye ehtiyac olmur

builder.Services.AddDbContext<AppDbContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
}
); ///////// proektin ayaga qalxanda getsin sql qosulsun



builder.Services.AddDefaultIdentity<User>()
	.AddRoles<IdentityRole>()
	.AddEntityFrameworkStores<AppDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
	{
		options.LoginPath = "/Auth/Login";
	}
);

builder.Services.Configure<IdentityOptions>(options =>
{
	options.Password.RequireNonAlphanumeric = false; //butwn simvollari qebul etmek ucun
	options.Password.RequiredLength = 6; //uzunlugu teyin etmek ucun
	options.Password.RequireDigit = false; //regem olmayada biler, mecbur deyil eger false yazilibsa 
	options.Password.RequireUppercase = false; //boyuk herfler mecburi deyil
	options.Lockout.MaxFailedAccessAttempts = 5; //eger 5 defe sehv daxil etdise
	options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); //bloklama vaxtı

});

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


app.UseEndpoints(endpoints =>
{
	endpoints.MapControllerRoute(
	  name: "areas",
	  pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
	);
});     /////////// Admin panele kecmek ucun /admin




app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
